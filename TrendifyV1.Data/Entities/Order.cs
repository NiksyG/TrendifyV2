namespace TrendifyV1.Data.Entities
{
    public class Order
    {
        public int Id { get; set; }

        public Guid UserId { get; set; }
        public ApplicationUser User { get; set; }

        public DateTime OrderDate { get; set; }
        public decimal TotalPrice { get; set; }

        public ICollection<OrderDetail> OrderDetails { get; set; }
    }
}
