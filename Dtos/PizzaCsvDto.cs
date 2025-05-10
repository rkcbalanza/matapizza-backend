using CsvHelper.Configuration.Attributes;

namespace MataPizza.Backend.Dtos
{
    public class PizzaCsvDto
    {
        [Name("pizza_id")]
        public string PizzaId { get; set; }
        [Name("pizza_type_id")]
        public string PizzaTypeId { get; set; }
        [Name("size")]
        public string Size { get; set; }
        [Name("price")]
        public decimal Price { get; set; }
    }
}
