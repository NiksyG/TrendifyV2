using TrendifyV1.Data.Entities;

namespace TrendifyV1.ViewModels.Product
{
    public class ProductCreateViewModel
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public decimal Price { get; set; }

        public string ImageUrl { get; set; }

        public int CategoryId { get; set; }

        public IEnumerable<TrendifyV1.Data.Entities.Category>? Categories { get; set; }
        public List<ProductSize> Sizes { get; set; } = new();
    }
}