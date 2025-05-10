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
            // Start with base query
            IQueryable<Order> query = _context.Orders;

            // Apply search filter if search term provided
            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(o => o.OrderId.ToString().Contains(search));
            }

            // Apply range date filter if provided
            // Apply start date
            if (!string.IsNullOrWhiteSpace(startDate) && DateTime.TryParse(startDate, out DateTime start))
            {
                query = query.Where(o => o.OrderDate >= start);
            }
            // Apply end date
            if (!string.IsNullOrWhiteSpace(endDate) && DateTime.TryParse(endDate, out DateTime end))
            {
                query = query.Where(o => o.OrderDate <= end);
            }


            // Apply price range filter if provided
            // Apply min range
            if (minPrice > 0)
            {
                query = query.Where(o => _context.OrderDetails
                    .Where(od => od.OrderId == o.OrderId)
                    .Sum(od => od.Quantity * od.Pizza.Price) >= minPrice);
            }
            // Apply max range
            if (maxPrice > 0)
            {
                query = query.Where(o => _context.OrderDetails
                    .Where(od => od.OrderId == o.OrderId)
                    .Sum(od => od.Quantity * od.Pizza.Price) <= maxPrice);
            }

            // Get total count for pagination
            var totalCount = await query.CountAsync();
            var totalSales = await query.Where(o => o.OrderId == o.OrderId)
                .SumAsync(o => _context.OrderDetails
                    .Where(od => od.OrderId == o.OrderId)
                    .Sum(od => od.Quantity * od.Pizza.Price));
            var totalItems = await query.Where(o => o.OrderId == o.OrderId)
                .SumAsync(o => _context.OrderDetails
                    .Where(od => od.OrderId == o.OrderId)
                    .Sum(od => od.Quantity));
            // Validate page number and size
            var orders = await query
                .Include(o => o.OrderDetails) // Include related OrderDetails data
                .Skip((page - 1) * pageSize) // Skip the previous pages
                .Take(pageSize) // Take the current page size
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
            return Ok(new { totalCount, totalSales, totalItems, orders });
        }

        // This endpoint retrieves a specific Order by its ID.
        [HttpGet("{id}")]
        public async Task<ActionResult<OrderDto>> GetOrderById(int id)
        {
            var order = await _context.Orders
                .Include(o => o.OrderDetails) // Include related OrderDetails data
                .Where(o => o.OrderId == id)
                .Select(o => new OrderDto
                {
                    OrderId = o.OrderId,
                    OrderDate = o.OrderDate.ToString("yyyy-MM-dd"),
                    OrderTime = o.OrderTime.ToString(@"hh\:mm\:ss"),
                    TotalItems = _context.OrderDetails.Where(od => od.OrderId == o.OrderId).Sum(od => od.Quantity), // Sum quantities for total items
                    TotalPrice = _context.OrderDetails.Where(od => od.OrderId == o.OrderId)
                                              .Sum(od => od.Quantity * od.Pizza.Price),
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
