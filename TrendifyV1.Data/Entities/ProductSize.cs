namespace TrendifyV1.Data.Entities
{
    public class ProductSize
    {
        public int Id { get; set; }

        public int ProductId { get; set; }
        public Product Product { get; set; }

        public string Size { get; set; }   
        public int Quantity { get; set; }
    }

}
