namespace TrendifyV1.Data.Entities
{
    public class OrderDetail
    {
        public int Id { get; set; }

        public int OrderId { get; set; }
        public Order Order { get; set; }

        public int ProductSizeId { get; set; }
        public ProductSize ProductSize { get; set; }

        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
}
