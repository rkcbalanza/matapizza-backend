using MataPizza.Backend.Data;
using MataPizza.Backend.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MataPizza.Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderDetailsController : ControllerBase
    {
        private readonly MataPizzaDbContext _context;
        public OrderDetailsController(MataPizzaDbContext context)
        {
            _context = context;
        }

        // This controller is responsible for handling requests related to OrderDetails.
        // This endpoint retrieves all OrderDetails from the database.
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderDetailDto>>> GetAllOrderDetails()
        {
            var orderDetails = await _context.OrderDetails
                .Include(od => od.Pizza) // Include related Pizza data
                .Include(od => od.Order) // Include related Order data
                .Select(od => new OrderDetailDto
                {
                    OrderDetailId = od.OrderDetailId,
                    OrderId = od.OrderId,
                    PizzaId = od.PizzaId,
                    Quantity = od.Quantity,
                    PizzaTypeName = od.Pizza.PizzaType.Name,
                    Size = od.Pizza.Size,
                    PriceEach = od.Pizza.Price, // Price of each pizza
                    TotalPrice = od.Quantity * od.Pizza.Price, // Calculate total price
                })
                .ToListAsync();
            return Ok(orderDetails);
        }

        // This endpoint retrieves a specific OrderDetail by its ID.
        [HttpGet("{id}")]
        public async Task<ActionResult<OrderDetailDto>> GetOrderDetailById(int id)
        {
            var orderDetail = await _context.OrderDetails
                .Include(od => od.Pizza) // Include related Pizza data
                .Include(od => od.Order) // Include related Order data  
                .Where(od => od.OrderDetailId == id)
                .Select(od => new OrderDetailDto
                {
                    OrderDetailId = od.OrderDetailId,
                    OrderId = od.OrderId,
                    PizzaId = od.PizzaId,
                    Quantity = od.Quantity,
                    PizzaTypeName = od.Pizza.PizzaType.Name,
                    Size = od.Pizza.Size,
                    PriceEach = od.Pizza.Price, // Price of each pizza
                    TotalPrice = od.Quantity * od.Pizza.Price, // Calculate total price
                })
                .FirstOrDefaultAsync();

            // Return 404 Not Found if the OrderDetail is not found
            if (orderDetail == null)
            {
                return NotFound($"Order detail with ID {id} not found.");
            }

            // Return the OrderDetail data
            return Ok(orderDetail);
        }
    }
}
