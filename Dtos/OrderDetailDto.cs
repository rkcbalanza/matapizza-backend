namespace MataPizza.Backend.Dtos
{
    public class OrderDetailDto
    {
        public int OrderDetailId { get; set; }
        public int OrderId { get; set; }
        public string PizzaId { get; set; }
        public string PizzaTypeName { get; set; }
        public string Size { get; set; }
        public int Quantity { get; set; }
        public decimal PriceEach { get; set; }
        public decimal TotalPrice { get; set; }
    }
}
