using MataPizza.Backend.Data;
using MataPizza.Backend.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MataPizza.Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CsvImportController : ControllerBase
    {
        private readonly CsvImporter _csvImporter;
        private readonly MataPizzaDbContext _context;
        public CsvImportController(CsvImporter csvImporter,MataPizzaDbContext context)
        {
            _csvImporter = csvImporter;
            _context = context;
        }

        [HttpPost("import-all")]
        public async Task<IActionResult> ImportAll()
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Import PizzaTypes, Pizzas, Orders, and OrderDetails from CSV files
                _csvImporter.ImportPizzaTypes("ImportData/pizza_types.csv");
                _csvImporter.ImportPizzas("ImportData/pizzas.csv");

                // We need to SET IDENTITY_INSERT ON for Orders and OrderDetails
                // because we are importing data with specific IDs
                // and we don't want SQL Server to auto-generate them.
                // We turn it OFF after each import for safety.
                await _context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT Orders ON");
                _csvImporter.ImportOrders("ImportData/orders.csv");
                await _context.SaveChangesAsync();
                await _context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT Orders OFF");

                await _context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT OrderDetails ON");
                _csvImporter.ImportOrderDetails("ImportData/order_details.csv");
                await _context.SaveChangesAsync();
                await _context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT OrderDetails OFF");

                await transaction.CommitAsync();

                return Ok("All CSV files imported successfully.");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, $"Import failed: {ex.Message}\n{ex.InnerException?.Message}");
            }
        }
    }
}
