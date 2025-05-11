

namespace MataPizza.Backend.Dtos
{
    public class OrderDto
    {
        public int OrderId { get; set; }
        public string OrderDate { get; set; }
        public string OrderTime { get; set; }
        public int TotalItems { get; set; }
        public decimal TotalPrice { get; set; }
        public List<OrderDetailDto> OrderDetails { get; set; }
    }
}
