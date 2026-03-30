using TrendifyV1.ViewModels.CategoryViewModels;
using TrendifyV1.ViewModels.ProductViewModels;

namespace TrendifyV1.Core.Interfaces
{
    public interface IProductService
    {
        Task<IEnumerable<ProductListViewModel>> GetAllProductsAsync();
        Task<ProductFormViewModel> GetProductForCreateAsync();
        Task CreateProductAsync(ProductFormViewModel model);
        Task<ProductFormViewModel> GetProductForEditAsync(Guid id);
        Task<bool> UpdateProductAsync(ProductFormViewModel model);
        Task<bool> DeleteProductAsync(Guid id);
        Task<IEnumerable<CategoryListViewModel>> GetCategoriesForDropdownAsync();
        Task<ProductDetailsViewModel?> GetProductDetailsAsync(Guid id);
    }
}
