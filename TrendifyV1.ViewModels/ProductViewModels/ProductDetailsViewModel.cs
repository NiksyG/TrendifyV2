using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrendifyV1.ViewModels.ProductViewModels
{
    public class ProductDetailsViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public decimal Price { get; set; }
        public string ImageUrl { get; set; } = null!;
        public string CategoryName { get; set; } = null!;
        public List<ProductSizeViewModel> AvailableSizes { get; set; } = new();

        public class ProductSizeViewModel
        {
            public int Id { get; set; }
            public string Size { get; set; } = null!;
            public int Quantity { get; set; }
        }
    }
}
