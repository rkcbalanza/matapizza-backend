using CsvHelper.Configuration.Attributes;

namespace MataPizza.Backend.Dtos
{
    public class PizzaTypeCsvDto
    {
        [Name("pizza_type_id")]
        public string PizzaTypeId { get; set; }
        [Name("name")]
        public string Name { get; set; }
        [Name("category")]
        public string Category { get; set; }
        [Name("ingredients")]
        public string Ingredients { get; set; }
    }
}
