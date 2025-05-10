using MataPizza.Backend.Data;
using MataPizza.Backend.Dtos;
using MataPizza.Backend.Models;
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

        // This endpoint retrieves all PizzaTypes from the database with pagination support.
        [HttpGet("paginated")]
        public async Task<ActionResult> GetPaginatedPizzaTypes(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string search = "",
            [FromQuery] string category = "")
        {
            // Start with base query
            IQueryable<PizzaType> query = _context.PizzaTypes;

            // Apply search filter if search term provided
            if (!string.IsNullOrWhiteSpace(search))
            {
                string searchLower = search.ToLower();
                query = query.Where(pt =>
                    pt.Name.ToLower().Contains(searchLower) ||
                    pt.Category.ToLower().Contains(searchLower) ||
                    pt.Ingredients.ToLower().Contains(searchLower));
            }

            // Apply category filter if provided
            if (!string.IsNullOrWhiteSpace(category))
            {
                query = query.Where(pt => pt.Category.Equals(category));
            }

            // Get total count for pagination
            var totalCount = await query.CountAsync();

            // Apply pagination and projection to DTO
            var pizzaTypes = await query
                .OrderBy(pt => pt.Name)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(pt => new PizzaTypeDto
                {
                    PizzaTypeId = pt.PizzaTypeId.ToString(),
                    Name = pt.Name,
                    Category = pt.Category,
                    Ingredients = pt.Ingredients
                })
                .ToListAsync();

            return Ok(new { totalCount, pizzaTypes });
        }


        // This endpoint retrieves a specific PizzaType by its ID.
        [HttpGet("{id}")]
        public async Task<ActionResult<PizzaTypeDto>> GetPizzaTypeById(string id)
        {
            var pizzaType = await _context.PizzaTypes
                .Include(pt => pt.Pizzas) // Include related Pizza data
                .Where(pt => pt.PizzaTypeId == id)
                .Select(pt => new PizzaTypeDto
                {
                    PizzaTypeId = pt.PizzaTypeId,
                    Name = pt.Name,
                    Category = pt.Category,
                    Ingredients = pt.Ingredients,
                    Pizzas = pt.Pizzas.Select(p => new PizzaDto
                    {
                        Size = p.Size,
                        Price = p.Price
                    }).ToList()
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

        // This endpoint retrieves PizzaType Categories only,
        // will be used for filtering in the frontend.
        [HttpGet("category")]
        public async Task<ActionResult<IEnumerable<string>>> GetPizzaTypeCategories()
        {
            var categories = await _context.PizzaTypes
                .Select(pt => pt.Category)
                .Distinct()
                .ToListAsync();

            return Ok(categories);
        }

    }
}
