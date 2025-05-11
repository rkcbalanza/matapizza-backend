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
            // Validate page number and size
            if (page < 1) page = 1;
            if (pageSize < 1 || pageSize > 100) pageSize = 10;

            // Normalize search term
            string searchLower = search?.ToLower().Trim() ?? "";
            bool hasSearchTerm = !string.IsNullOrWhiteSpace(searchLower);

            // Normalize category once
            string categoryLower = category?.ToLower().Trim() ?? "";
            bool hasCategory = !string.IsNullOrWhiteSpace(categoryLower);

            // Start with base query
            IQueryable<PizzaType> query = _context.PizzaTypes;

            // Apply filters conditionally
            if (hasSearchTerm)
            {
                // Use compiled LINQ expression for better performance with EF Core
                query = query.Where(pt =>
                    EF.Functions.Like(pt.Name.ToLower(), $"%{searchLower}%") ||
                    EF.Functions.Like(pt.Category.ToLower(), $"%{searchLower}%") ||
                    EF.Functions.Like(pt.Ingredients.ToLower(), $"%{searchLower}%"));
            }

            if (hasCategory)
            {
                // Use EF.Functions.Like for better database translation
                query = query.Where(pt => EF.Functions.Like(pt.Category.ToLower(), categoryLower));
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
            // Validate ID
            if (string.IsNullOrWhiteSpace(id))
            {
                return BadRequest("PizzaType ID cannot be null or empty.");
            }
            try
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
            catch (Exception ex)
            {
                // Return 500 Internal Server Error for any unexpected exceptions
                return StatusCode(500, $"Internal server error: {ex.Message}");

            }
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
