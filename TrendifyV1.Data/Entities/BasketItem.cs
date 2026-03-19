namespace TrendifyV1.Data.Entities
{
    public class BasketItem
    {
        public int Id { get; set; }

        public Guid UserId { get; set; }
        public ApplicationUser User { get; set; }

        public int ProductSizeId { get; set; }
        public ProductSize ProductSize { get; set; }

        public int Quantity { get; set; }
    }

}
