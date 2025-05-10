using CsvHelper.Configuration.Attributes;

namespace MataPizza.Backend.Dtos
{
    public class OrderCsvDto
    {
        [Name("order_id")]
        public int OrderId { get; set; }
        [Name("date")]
        public DateTime OrderDate { get; set; }
        [Name("time")]
        public TimeSpan OrderTime { get; set; }
    }
}
