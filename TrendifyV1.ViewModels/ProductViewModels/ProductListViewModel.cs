namespace TrendifyV1.ViewModels.ProductViewModels
{
    public class ProductListViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string ImageUrl { get; set; }
        public string CategoryName { get; set; }

        public string DisplayImageUrl => string.IsNullOrWhiteSpace(ImageUrl)
                    ? "https://dummyimage.com/300x400/dee2e6/6c757d.jpg&text=No+Image"
                    : ImageUrl;
        public List<ProductSizeViewModel> AvailableSizes { get; set; } = new List<ProductSizeViewModel>();

        public class ProductSizeViewModel
        {
            public int Id { get; set; }
            public string Size { get; set; }
            public int Quantity { get; set; }
        }
    }

}
