using TrendifyV1.Core.Interfaces;
using TrendifyV1.Data.Entities;
using TrendifyV1.ViewModels.CategoryViewModels;
using TrendifyV1.ViewModels.ProductViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrendifyV1.Data;
using TrendifyV1.Data.Entities;
using static TrendifyV1.ViewModels.ProductViewModels.ProductCreateViewModel;

namespace TrendifyV1.Core.Implementations
{
    public class ProductService : IProductService
    {
        private readonly TrendifyV1DbContext _context;

        public ProductService(TrendifyV1DbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<CategoryListViewModel>> GetCategoriesForDropdownAsync()
        {
            return await _context.Categories
                .Select(c => new CategoryListViewModel
                {
                    Id = c.Id,
                    Name = c.Name
                }).ToListAsync();
        }

        public async Task<ProductFormViewModel> GetProductForCreateAsync()
        {
            return new ProductFormViewModel
            {
                Categories = await GetCategoriesForDropdownAsync(),
                Sizes = new List<ProductSizeInputModel>
                {
                    new() { Size = "S" },
                    new() { Size = "M" },
                    new() { Size = "L" }
                }
            };
        }

        public async Task CreateProductAsync(ProductFormViewModel model)
        {
            var product = new Product
            {
                Id = Guid.NewGuid(),
                Name = model.Name,
                Description = model.Description,
                Price = model.Price,
                ImageUrl = model.ImageUrl,
                CategoryId = model.CategoryId,
                ProductSizes = model.Sizes.Select(s => new ProductSize
                {
                    Size = s.Size,
                    Quantity = s.Quantity
                }).ToList()
            };

            _context.Products.Add(product);
            await _context.SaveChangesAsync();
        }

        public async Task<ProductFormViewModel> GetProductForEditAsync(Guid id)
        {
            var product = await _context.Products
                .Include(p => p.ProductSizes)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null) return null;

            return new ProductFormViewModel
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                ImageUrl = product.ImageUrl,
                CategoryId = product.CategoryId,
                Categories = await GetCategoriesForDropdownAsync(),
                Sizes = product.ProductSizes.Select(s => new ProductSizeInputModel
                {
                    Id = s.Id,
                    Size = s.Size,
                    Quantity = s.Quantity
                }).ToList()
            };
        }

        public async Task<bool> UpdateProductAsync(ProductFormViewModel model)
        {
            var product = await _context.Products
                .Include(p => p.ProductSizes)
                .FirstOrDefaultAsync(p => p.Id == model.Id);

            if (product == null) return false;

            product.Name = model.Name;
            product.Description = model.Description;
            product.Price = model.Price;
            product.ImageUrl = model.ImageUrl;
            product.CategoryId = model.CategoryId;

            foreach (var sizeInput in model.Sizes)
            {
                var existingSize = product.ProductSizes
                    .FirstOrDefault(s => s.Size == sizeInput.Size);

                if (existingSize != null)
                {
                    existingSize.Quantity = sizeInput.Quantity;
                }
                else
                {
                    product.ProductSizes.Add(new ProductSize
                    {
                        Size = sizeInput.Size,
                        Quantity = sizeInput.Quantity
                    });
                }
            }

            await _context.SaveChangesAsync();
            return true;
        }


        public async Task<bool> DeleteProductAsync(Guid id)
        {
            var product = await _context.Products.FindAsync(id);

            if (product == null) return false;

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<ProductListViewModel>> GetAllProductsAsync()
        {
            var da = await _context.Products
                .Select(p => new ProductListViewModel
                {
                    Id = p.Id,
                    Name = p.Name,
                    Price = p.Price,
                    ImageUrl = p.ImageUrl,
                    CategoryName = p.Category.Name,
                    AvailableSizes = p.ProductSizes.Select(s => new ProductListViewModel.ProductSizeViewModel
                    {
                        Id = s.Id,
                        Size = s.Size,
                        Quantity = s.Quantity
                    }).ToList()
                })
                .ToListAsync();

            return da;
        }

        public async Task<ProductDetailsViewModel?> GetProductDetailsAsync(Guid id)
        {
            var product = await _context.Products
                .Where(p => p.Id == id)
                .Select(p => new ProductDetailsViewModel
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    Price = p.Price,
                    ImageUrl = p.ImageUrl,
                    CategoryName = p.Category.Name,
                    AvailableSizes = p.ProductSizes.Select(s => new ProductDetailsViewModel.ProductSizeViewModel
                    {
                        Id = s.Id,
                        Size = s.Size,
                        Quantity = s.Quantity
                    }).ToList()
                })
                .FirstOrDefaultAsync();

            return product;
        }
    }
}
