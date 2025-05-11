namespace MataPizza.Backend.Models
{
    public class OrderDetail
    {
        public int OrderDetailId { get; set; }
        public int OrderId { get; set; }
        public string PizzaId { get; set; }
        public int Quantity { get; set; }
        public Order Order { get; set; }
        public Pizza Pizza { get; set; }
    }
}
