using MataPizza.Backend.Data;
using MataPizza.Backend.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MataPizza.Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PizzasController : ControllerBase
    {
        private readonly MataPizzaDbContext _context;
        public PizzasController(MataPizzaDbContext context)
        {
            _context = context;
        }

        // This controller is responsible for handling requests related to Pizzas.
        // This endpoint retrieves all Pizzas from the database.
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PizzaDto>>> GetAllPizzas()
        {
            var pizzas = await _context.Pizzas
                .Include(p => p.PizzaType) // Include related PizzaType data
                .Select(p =>  new PizzaDto
                {
                    PizzaId = p.PizzaId,
                    PizzaTypeId = p.PizzaTypeId,
                    Size = p.Size,
                    Price = p.Price,
                })
                .ToListAsync();

            return Ok(pizzas);
        }

        // This endpoint retrieves a specific Pizza by its ID.
        [HttpGet("{id}")]
        public async Task<ActionResult<PizzaDto>> GetPizzaById(string id)
        {
            // Validate ID
            if (string.IsNullOrWhiteSpace(id))
            {
                return BadRequest("Pizza ID cannot be null or empty.");
            }
            try
            {
                var pizza = await _context.Pizzas
                .Include(p => p.PizzaType) // Include related PizzaType data
                .Where(p => p.PizzaId == id)
                .Select(p => new PizzaDto
                {
                    PizzaId = p.PizzaId,
                    PizzaTypeId = p.PizzaTypeId,
                    Size = p.Size,
                    Price = p.Price,
                })
                .FirstOrDefaultAsync();

                // Return 404 Not Found if the Pizza is not found
                if (pizza == null)
                {
                    return NotFound($"Pizza with ID '{id}' not found.");
                }

                // Return the found Pizza
                return Ok(pizza);
            }
            catch (Exception ex)
            {
                // Return 500 Internal Server Error with exception message
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
    }
}
