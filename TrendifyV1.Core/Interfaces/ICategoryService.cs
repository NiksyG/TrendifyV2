using TrendifyV1.ViewModels.CategoryViewModels;

namespace TrendifyV1.Core.Interfaces
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryListViewModel>> GetAllCategoriesAsync();
        Task<CategoryFormViewModel> GetCategoryByIdAsync(int id);
        Task CreateCategoryAsync(CategoryFormViewModel model);
        Task UpdateCategoryAsync(CategoryFormViewModel model);
        Task<bool> DeleteCategoryAsync(int id);
    }
}
