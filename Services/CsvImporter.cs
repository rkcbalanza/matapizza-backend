using CsvHelper;
using MataPizza.Backend.Data;
using MataPizza.Backend.Dtos;
using MataPizza.Backend.Models;
using System.Globalization;

namespace MataPizza.Backend.Services
{
    public class CsvImporter
    {
        private readonly MataPizzaDbContext _context;

        public CsvImporter(MataPizzaDbContext context)
        {
            _context = context;
        }

        // Method to import records on PizzaTypes table from a CSV file
        public void ImportPizzaTypes(string filePath)
        {
            using var reader = new StreamReader(filePath);
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
            var records = csv.GetRecords<PizzaTypeCsvDto>().ToList();

            foreach (var record in records)
            {
                if (_context.PizzaTypes.Any(pt => pt.PizzaTypeId == record.PizzaTypeId)) continue;

                _context.PizzaTypes.Add(new PizzaType
                {
                    PizzaTypeId = record.PizzaTypeId,
                    Name = record.Name,
                    Category = record.Category,
                    Ingredients = record.Ingredients
                });
            }
            _context.SaveChanges();
        }

        // Method to import records on Pizzas table from a CSV file
        public void ImportPizzas(string filePath)
        {
            using var reader = new StreamReader(filePath);
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
            var records = csv.GetRecords<PizzaCsvDto>().ToList();

            foreach (var record in records)
            {
                if (_context.Pizzas.Any(p => p.PizzaId == record.PizzaId)) continue;
                _context.Pizzas.Add(new Pizza
                {
                    PizzaId = record.PizzaId,
                    PizzaTypeId = record.PizzaTypeId,
                    Size = record.Size,
                    Price = record.Price
                });
            }
            _context.SaveChanges();
        }

        // Method to import records on Orders table from a CSV file
        public void ImportOrders(string filePath)
        {
            using var reader = new StreamReader(filePath);
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
            var records = csv.GetRecords<OrderCsvDto>().ToList();
            foreach (var record in records)
            {
                if (_context.Orders.Any(o => o.OrderId == record.OrderId)) continue;
                _context.Orders.Add(new Order
                {
                    OrderId = record.OrderId,
                    OrderDate = record.OrderDate,
                    OrderTime = record.OrderTime
                });
            }
        }

        // Method to import records on OrderDetails table from a CSV file
        public void ImportOrderDetails(string filePath)
        {
            using var reader = new StreamReader(filePath);
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
            var records = csv.GetRecords<OrderDetailCsvDto>().ToList();
            foreach (var record in records)
            {
                if (_context.OrderDetails.Any(od => od.OrderDetailId == record.OrderDetailId)) continue;
                _context.OrderDetails.Add(new OrderDetail
                {
                    OrderDetailId = record.OrderDetailId,
                    OrderId = record.OrderId,
                    PizzaId = record.PizzaId,
                    Quantity = record.Quantity
                });
            }
            _context.SaveChanges();
        }
    }
}
