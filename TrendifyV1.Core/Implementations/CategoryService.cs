using Microsoft.EntityFrameworkCore;
using TrendifyV1.Core.Interfaces;
using TrendifyV1.Data.Entities;
using TrendifyV1.ViewModels.CategoryViewModels;

namespace TrendifyV1.Core
{
    public class CategoryService(
        TrendifyV1DbContext context) 
        : ICategoryService
    {
        public async Task<IEnumerable<CategoryListViewModel>> GetAllCategoriesAsync()
        {
            return await context.Categories
                .Select(c => new CategoryListViewModel
                {
                    Id = c.Id,
                    Name = c.Name,
                    ProductsCount = c.Products.Count
                })
                .ToListAsync();
        }

        public async Task<CategoryFormViewModel> GetCategoryByIdAsync(int id)
        {
            var category = await context.Categories.FindAsync(id);
            if (category == null) return null;

            return new CategoryFormViewModel
            {
                Id = category.Id,
                Name = category.Name
            };
        }

        public async Task CreateCategoryAsync(CategoryFormViewModel model)
        {
            var category = new Category
            {
                Name = model.Name
            };

            context.Categories.Add(category);
            await context.SaveChangesAsync();
        }

        public async Task UpdateCategoryAsync(CategoryFormViewModel model)
        {
            var category = await context.Categories.FindAsync(model.Id);
            if (category != null)
            {
                category.Name = model.Name;
                await context.SaveChangesAsync();
            }
        }

        public async Task<bool> DeleteCategoryAsync(int id)
        {
            var category = await context.Categories
                .Include(c => c.Products)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (category == null) return false;

            if (category.Products.Any()) return false;

            context.Categories.Remove(category);
            await context.SaveChangesAsync();
            return true;
        }
    }
}