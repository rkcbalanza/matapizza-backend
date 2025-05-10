namespace MataPizza.Backend.Models
{
    public class Order
    {
        public int OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public TimeSpan OrderTime { get; set; }

        public ICollection<OrderDetail> OrderDetails { get; set; }
    }
}
