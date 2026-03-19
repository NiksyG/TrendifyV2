using TrendifyV1.Data.Entities;
using TrendifyV1.ViewModels.Product;

namespace TrendifyV1.ViewModels.Product
{
    public class ProductFormViewModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public decimal Price { get; set; }

        public string ImageUrl { get; set; }

        public int CategoryId { get; set; }

        public IEnumerable<TrendifyV1.Data.Entities.Category>? Categories { get; set; }
        public List<ProductSizeInputModel> Sizes { get; set; } = new();
    }
}