using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System;
using System.Linq;
using System.Threading.Tasks;
using TrendifyV1.Core;
using TrendifyV1.Data;
using TrendifyV1.Data.Entities;
using TrendifyV1.ViewModels.CategoryViewModels;

namespace TrendifyV1.Core.Tests
{
    [TestFixture]
    public class CategoryServiceTests
    {
        private TrendifyV1DbContext _context;
        private CategoryService _categoryService;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<TrendifyV1DbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new TrendifyV1DbContext(options);
            _categoryService = new CategoryService(_context);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Test]
        public async Task GetAllCategoriesAsync_ReturnsMappedCategories()
        {
            // Arrange
            var category1 = new Category { Id = 1, Name = "Category 1", Products = new List<Product>() };
            var category2 = new Category { Id = 2, Name = "Category 2", Products = new List<Product>() };
            
            category1.Products.Add(new Product { Id = Guid.NewGuid(), Name = "P1", Price = 10, Description = "", CategoryId = 1, ImageUrl = "i.jpg" });

            _context.Categories.AddRange(category1, category2);
            await _context.SaveChangesAsync();

            // Act
            var result = (await _categoryService.GetAllCategoriesAsync()).ToList();

            // Assert
            Assert.That(result.Count, Is.EqualTo(2));
            var mappedCategory1 = result.First(c => c.Id == 1);
            Assert.That(mappedCategory1.ProductsCount, Is.EqualTo(1));
            Assert.That(mappedCategory1.Name, Is.EqualTo("Category 1"));
            
            var mappedCategory2 = result.First(c => c.Id == 2);
            Assert.That(mappedCategory2.ProductsCount, Is.EqualTo(0));
        }

        [Test]
        public async Task GetCategoryByIdAsync_ValidId_ReturnsCategory()
        {
            // Arrange
            _context.Categories.Add(new Category { Products = new List<Product>(), Id = 1, Name = "Test Category" });
            await _context.SaveChangesAsync();

            // Act
            var result = await _categoryService.GetCategoryByIdAsync(1);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.EqualTo(1));
            Assert.That(result.Name, Is.EqualTo("Test Category"));
        }

        [Test]
        public async Task GetCategoryByIdAsync_InvalidId_ReturnsNull()
        {
            // Act
            var result = await _categoryService.GetCategoryByIdAsync(999);

            // Assert
            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task CreateCategoryAsync_AddsCategory()
        {
            // Arrange
            var model = new CategoryFormViewModel { Name = "New Category" };

            // Act
            await _categoryService.CreateCategoryAsync(model);

            // Assert
            var categories = await _context.Categories.ToListAsync();
            Assert.That(categories.Count, Is.EqualTo(1));
            Assert.That(categories[0].Name, Is.EqualTo("New Category"));
        }

        [Test]
        public async Task UpdateCategoryAsync_ValidId_UpdatesCategory()
        {
            // Arrange
            _context.Categories.Add(new Category { Products = new List<Product>(), Id = 1, Name = "Old Name" });
            await _context.SaveChangesAsync();
            
            var model = new CategoryFormViewModel { Id = 1, Name = "New Name" };

            // Act
            await _categoryService.UpdateCategoryAsync(model);

            // Assert
            var category = await _context.Categories.FindAsync(1);
            Assert.That(category.Name, Is.EqualTo("New Name"));
        }

        [Test]
        public async Task UpdateCategoryAsync_InvalidId_DoesNothing()
        {
            // Arrange
            _context.Categories.Add(new Category { Products = new List<Product>(), Id = 1, Name = "Old Name" });
            await _context.SaveChangesAsync();
            
            var model = new CategoryFormViewModel { Id = 999, Name = "New Name" };

            // Act
            await _categoryService.UpdateCategoryAsync(model);

            // Assert
            var category = await _context.Categories.FindAsync(1);
            Assert.That(category.Name, Is.EqualTo("Old Name"));
        }

        [Test]
        public async Task DeleteCategoryAsync_ValidIdWithoutProducts_DeletesCategoryReturnsTrue()
        {
            // Arrange
            _context.Categories.Add(new Category { Products = new List<Product>(), Id = 1, Name = "Test" });
            await _context.SaveChangesAsync();

            // Act
            var result = await _categoryService.DeleteCategoryAsync(1);

            // Assert
            Assert.That(result, Is.True);
            Assert.That(await _context.Categories.CountAsync(), Is.EqualTo(0));
        }

        [Test]
        public async Task DeleteCategoryAsync_InvalidId_ReturnsFalse()
        {
            // Act
            var result = await _categoryService.DeleteCategoryAsync(999);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public async Task DeleteCategoryAsync_CategoryHasProducts_ReturnsFalse()
        {
            // Arrange
            var category = new Category { Id = 1, Name = "Test", Products = new List<Product>() };
            category.Products.Add(new Product { Id = Guid.NewGuid(), Name = "Test Prod", Description = "", Price = 1, ImageUrl = "test", CategoryId = 1 });
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            // Act
            var result = await _categoryService.DeleteCategoryAsync(1);

            // Assert
            Assert.That(result, Is.False);
            Assert.That(await _context.Categories.CountAsync(), Is.EqualTo(1));
        }
    }
}
