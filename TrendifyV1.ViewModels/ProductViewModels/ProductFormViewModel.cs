using TrendifyV1.ViewModels.CategoryViewModels;

namespace TrendifyV1.ViewModels.ProductViewModels
{
    public class ProductFormViewModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public decimal Price { get; set; }

        public string? ImageUrl { get; set; }

        public int CategoryId { get; set; }

        public IEnumerable<CategoryListViewModel>? Categories { get; set; }
        public List<ProductSizeInputModel> Sizes { get; set; } = new();
    }
}