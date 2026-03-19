namespace TrendifyV1.Data.Entities
{
    public class Favorite
    {
        public int Id { get; set; }

        public Guid UserId { get; set; }
        public ApplicationUser User { get; set; }

        public int ProductId { get; set; }
        public Product Product { get; set; }
    }

}
