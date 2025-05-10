using MataPizza.Backend.Data;
using MataPizza.Backend.Dtos;
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

        // This endpoint retrieves a specific Order by its ID.
        [HttpGet("{id}")]
        public async Task<ActionResult<OrderDto>> GetOrderById(int id)
        {
            var order = await _context.Orders
                .Include(o => o.OrderDetails) // Include related OrderDetails data
                .Select(o => new OrderDto
                {
                    OrderId = o.OrderId,
                    OrderDate = o.OrderDate.ToString("yyyy-MM-dd"),
                    OrderTime = o.OrderTime.ToString(@"hh\:mm\:ss"),
                    TotalItems = _context.OrderDetails.Where(od => od.OrderId == o.OrderId).Sum(od => od.Quantity), // Sum quantities for total items
                    TotalPrice = _context.OrderDetails.Where(od => od.OrderId == o.OrderId)
                                              .Sum(od => od.Quantity * od.Pizza.Price)
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
