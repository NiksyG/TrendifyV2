using TrendifyV1.Data.Entities;
using TrendifyV1.ViewModels.CategoryViewModels;

namespace TrendifyV1.ViewModels.ProductViewModels
{
    public class ProductCreateViewModel
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public decimal Price { get; set; }

        public string ImageUrl { get; set; }

        public int CategoryId { get; set; }

        public IEnumerable<CategoryListViewModel>? Categories { get; set; }
        public List<ProductSize> Sizes { get; set; } = new();
        public List<ProductSizeViewModel> AvailableSizes { get; set; } = new List<ProductSizeViewModel>();
        public class ProductSizeViewModel
        {
            public int Id { get; set; }
            public string Size { get; set; }
        }
    }
}