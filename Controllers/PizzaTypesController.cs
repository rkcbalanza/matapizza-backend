using MataPizza.Backend.Data;
using MataPizza.Backend.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MataPizza.Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PizzaTypesController : ControllerBase
    {
        private readonly MataPizzaDbContext _context;
        public PizzaTypesController(MataPizzaDbContext context)
        {
            _context = context;
        }

        // This controller is responsible for handling requests related to PizzaTypes.
        // This endpoint retrieves all PizzaTypes from the database.
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PizzaTypeDto>>> GetAllPizzaTypes()
        {
            var pizzaTypes = await _context.PizzaTypes
                .Select(pt => new PizzaTypeDto
                {
                    PizzaTypeId = pt.PizzaTypeId,
                    Name = pt.Name,
                    Category = pt.Category,
                    Ingredients = pt.Ingredients
                })
                .ToListAsync();
            return Ok(pizzaTypes);
        }

        // This endpoint retrieves a specific PizzaType by its ID.
        [HttpGet("{id}")]
        public async Task<ActionResult<PizzaTypeDto>> GetPizzaTypeById(string id)
        {
            var pizzaType = await _context.PizzaTypes
                .Where(pt => pt.PizzaTypeId == id)
                .Select(pt => new PizzaTypeDto
                {
                    PizzaTypeId = pt.PizzaTypeId,
                    Name = pt.Name,
                    Category = pt.Category,
                    Ingredients = pt.Ingredients
                })
                .FirstOrDefaultAsync();

            // Return 404 Not Found if the PizzaType is not found
            if (pizzaType == null)
            {
                return NotFound($"Pizza type with ID {id} not found.");
            }

            // Return the PizzaType details
            return Ok(pizzaType);
        }
    }
}
