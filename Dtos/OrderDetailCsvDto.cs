using CsvHelper.Configuration.Attributes;

namespace MataPizza.Backend.Dtos
{
    public class OrderDetailCsvDto
    {
        [Name("order_details_id")]
        public int OrderDetailId { get; set; }
        [Name("order_id")]
        public int OrderId { get; set; }
        [Name("pizza_id")]
        public string PizzaId { get; set; }
        [Name("quantity")]
        public int Quantity { get; set; }
    }
}
