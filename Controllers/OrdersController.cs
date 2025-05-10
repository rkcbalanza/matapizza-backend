using MataPizza.Backend.Data;
using MataPizza.Backend.Dtos;
using MataPizza.Backend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MataPizza.Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly MataPizzaDbContext _context;
        public OrdersController(MataPizzaDbContext context)
        {
            _context = context;
        }

        // This controller is responsible for handling requests related to orders.
        // This endpoint retrieves all Orders from the database.
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderDto>>> GetAllOrders()
        {
            var orders = await _context.Orders
                .Include(o => o.OrderDetails) // Include related OrderDetails data
                .Select(o => new OrderDto
                {
                    OrderId = o.OrderId,
                    OrderDate = o.OrderDate.ToString("yyyy-MM-dd"),
                    OrderTime = o.OrderTime.ToString(@"hh\:mm\:ss"),
                    TotalItems = _context.OrderDetails.Where(od => od.OrderId == o.OrderId).Sum(od => od.Quantity), // Sum quantities for total items
                    TotalPrice = _context.OrderDetails.Where(od => od.OrderId == o.OrderId)
                                              .Sum(od => od.Quantity * od.Pizza.Price) // Sum total price
                })
                .ToListAsync();
            return Ok(orders);
        }

        // This endpoint retrieves all Orders from the database with pagination support.
        [HttpGet("paginated")]
        public async Task<ActionResult> GetPaginatedOrders(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string search = "",
            [FromQuery] string startDate = "",
            [FromQuery] string endDate = "",
            [FromQuery] decimal minPrice = 0,
            [FromQuery] decimal maxPrice = 0)
        {
            // Validate page number and size
            if (page < 1) page = 1;
            if (pageSize < 1 || pageSize > 100) pageSize = 10;

            // Start with base query
            IQueryable<Order> query = _context.Orders
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Pizza);

            // Apply search filter if search term provided
            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(o => o.OrderId.ToString().Contains(search));
            }

            // Parsing DateTime strings to DateTime objects
            DateTime start = DateTime.MinValue;
            DateTime end = DateTime.MaxValue;

            if (!string.IsNullOrWhiteSpace(startDate) && DateTime.TryParse(startDate, out start))
            {
                // Start variable is already set
            }
            if (!string.IsNullOrWhiteSpace(endDate) && DateTime.TryParse(endDate, out end))
            {
                // Adjust to end of day for inclusive filtering
                end = end.Date.AddDays(1).AddTicks(-1);
            }

            // Apply date filters once
            query = query.Where(o => o.OrderDate >= start && o.OrderDate <= end);

            // Prepare an optimized projection for price calculations
            var ordersWithPrice = query.Select(o => new
            {
                Order = o,
                TotalPrice = o.OrderDetails.Sum(od => od.Quantity * od.Pizza.Price)
            });

            // Apply price range filters
            if (minPrice > 0)
            {
                ordersWithPrice = ordersWithPrice.Where(o => o.TotalPrice >= minPrice);
            }

            if (maxPrice > 0)
            {
                ordersWithPrice = ordersWithPrice.Where(o => o.TotalPrice <= maxPrice);
            }

            // Perform calculations for summary stats with optimized queries
            var totalCount = await ordersWithPrice.CountAsync();
            var totalSales = await ordersWithPrice.SumAsync(o => o.TotalPrice);
            var totalItems = await ordersWithPrice.SumAsync(o => o.Order.OrderDetails.Sum(od => od.Quantity));

            // Validate page number and size
            var orders = await ordersWithPrice
               .Skip((page - 1) * pageSize)
               .Take(pageSize)
               .Select(o => new OrderDto
               {
                   OrderId = o.Order.OrderId,
                   OrderDate = o.Order.OrderDate.ToString("yyyy-MM-dd"),
                   OrderTime = o.Order.OrderTime.ToString(@"hh\:mm\:ss"),
                   TotalItems = o.Order.OrderDetails.Sum(od => od.Quantity),
                   TotalPrice = o.TotalPrice,
                   OrderDetails = o.Order.OrderDetails.Select(od => new OrderDetailDto
                   {
                       PizzaTypeName = od.Pizza.PizzaType.Name,
                       Quantity = od.Quantity,
                       PriceEach = od.Pizza.Price,
                       TotalPrice = od.Quantity * od.Pizza.Price
                   }).ToList()
               })
               .ToListAsync();

            return Ok(new { totalCount, totalSales, totalItems, orders });
        }

        // This endpoint retrieves a specific Order by its ID.
        [HttpGet("{id}")]
        public async Task<ActionResult<OrderDto>> GetOrderById(int id)
        {
            var order = await _context.Orders
                .Where(o => o.OrderId == id)
                .Select(o => new OrderDto
                {
                    OrderId = o.OrderId,
                    OrderDate = o.OrderDate.ToString("yyyy-MM-dd"),
                    OrderTime = o.OrderTime.ToString(@"hh\:mm\:ss"),
                    TotalItems = o.OrderDetails.Sum(od => od.Quantity), // Sum quantities for total items
                    TotalPrice = o.OrderDetails.Sum(od => od.Quantity * od.Pizza.Price), // Sum total price
                    OrderDetails = o.OrderDetails.Select(od => new OrderDetailDto
                    {
                        Quantity = od.Quantity,
                        PizzaTypeName = od.Pizza.PizzaType.Name,
                        Size = od.Pizza.Size,
                        PriceEach = od.Pizza.Price, // Price of each pizza
                        TotalPrice = od.Quantity * od.Pizza.Price // Calculate total price
                    }).ToList()
                })
                .FirstOrDefaultAsync();

            // Return 404 Not Found if the Order is not found
            if (order == null)
            {
                return NotFound($"Order with ID {id} not found.");
            }
            return Ok(order);
        }
    }
}
