using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrendifyV1.Core.Implementations;
using TrendifyV1.Data;
using TrendifyV1.Data.Entities;
using TrendifyV1.ViewModels.ProductViewModels;

namespace TrendifyV1.Core.Tests
{
    [TestFixture]
    public class ProductServiceTests
    {
        private TrendifyV1DbContext _context;
        private ProductService _productService;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<TrendifyV1DbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new TrendifyV1DbContext(options);
            _productService = new ProductService(_context);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Test]
        public async Task GetCategoriesForDropdownAsync_ReturnsCategories()
        {
            // Arrange
            _context.Categories.AddRange(
                new Category { Id = 1, Name = "Cat 1" },
                new Category { Id = 2, Name = "Cat 2" }
            );
            await _context.SaveChangesAsync();

            // Act
            var result = (await _productService.GetCategoriesForDropdownAsync()).ToList();

            // Assert
            Assert.That(result.Count, Is.EqualTo(2));
            Assert.That(result[0].Name, Is.EqualTo("Cat 1"));
        }

        [Test]
        public async Task GetProductForCreateAsync_ReturnsFormWithCategoriesAndDefaultSizes()
        {
            // Arrange
            _context.Categories.Add(new Category { Id = 1, Name = "Cat 1" });
            await _context.SaveChangesAsync();

            // Act
            var result = await _productService.GetProductForCreateAsync();

            // Assert
            Assert.That(result.Categories.Count(), Is.EqualTo(1));
            Assert.That(result.Sizes.Count, Is.EqualTo(3));
            Assert.That(result.Sizes.Any(s => s.Size == "S"), Is.True);
            Assert.That(result.Sizes.Any(s => s.Size == "M"), Is.True);
            Assert.That(result.Sizes.Any(s => s.Size == "L"), Is.True);
        }

        [Test]
        public async Task CreateProductAsync_AddsProductAndSizes()
        {
            // Arrange
            var model = new ProductFormViewModel
            {
                Name = "New Product",
                Description = "Desc",
                Price = 50,
                ImageUrl = "img.jpg",
                CategoryId = 1,
                Sizes = new List<ProductSizeInputModel>
                {
                    new() { Size = "S", Quantity = 10 },
                    new() { Size = "M", Quantity = 20 }
                }
            };

            // Act
            await _productService.CreateProductAsync(model);

            // Assert
            var rootProduct = await _context.Products.Include(p => p.ProductSizes).FirstOrDefaultAsync();
            Assert.That(rootProduct, Is.Not.Null);
            Assert.That(rootProduct.Name, Is.EqualTo("New Product"));
            Assert.That(rootProduct.ProductSizes.Count, Is.EqualTo(2));
        }

        [Test]
        public async Task GetProductForEditAsync_ValidId_ReturnsFilledForm()
        {
            // Arrange
            var product = new Product
            {
                Id = Guid.NewGuid(),
                Name = "Edit me",
                Description = "", ImageUrl = "",
                CategoryId = 1,
                ProductSizes = new List<ProductSize>
                {
                    new() { Id = 1, Size = "M", Quantity = 5 }
                }
            };
            _context.Categories.Add(new Category { Id = 1, Name = "Cat 1" });
            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            // Act
            var result = await _productService.GetProductForEditAsync(product.Id);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Name, Is.EqualTo("Edit me"));
            Assert.That(result.Categories.Count(), Is.EqualTo(1));
            Assert.That(result.Sizes.Count, Is.EqualTo(1));
            Assert.That(result.Sizes.First().Size, Is.EqualTo("M"));
        }

        [Test]
        public async Task GetProductForEditAsync_InvalidId_ReturnsNull()
        {
            // Act
            var result = await _productService.GetProductForEditAsync(Guid.NewGuid());

            // Assert
            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task UpdateProductAsync_ValidId_UpdatesProductAndSizes()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var existingProduct = new Product
            {
                Id = productId,
                Name = "Old Name",
                Description = "", ImageUrl = "",
                ProductSizes = new List<ProductSize>
                {
                    new() { Size = "S", Quantity = 5 } // will be updated
                }
            };
            _context.Products.Add(existingProduct);
            await _context.SaveChangesAsync();

            var model = new ProductFormViewModel
            {
                Id = productId,
                Name = "New Name",
                Sizes = new List<ProductSizeInputModel>
                {
                    new() { Size = "S", Quantity = 10 }, // update existing
                    new() { Size = "M", Quantity = 15 }  // add new
                }
            };

            // Act
            var result = await _productService.UpdateProductAsync(model);

            // Assert
            Assert.That(result, Is.True);
            var updatedProduct = await _context.Products.Include(p => p.ProductSizes).FirstOrDefaultAsync();
            Assert.That(updatedProduct.Name, Is.EqualTo("New Name"));
            Assert.That(updatedProduct.ProductSizes.Count, Is.EqualTo(2));
            Assert.That(updatedProduct.ProductSizes.First(s => s.Size == "S").Quantity, Is.EqualTo(10));
            Assert.That(updatedProduct.ProductSizes.First(s => s.Size == "M").Quantity, Is.EqualTo(15));
        }

        [Test]
        public async Task UpdateProductAsync_InvalidId_ReturnsFalse()
        {
            // Act
            var result = await _productService.UpdateProductAsync(new ProductFormViewModel { Id = Guid.NewGuid() });

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public async Task DeleteProductAsync_ValidId_DeletesProductAndReturnsTrue()
        {
            // Arrange
            var productId = Guid.NewGuid();
            _context.Products.Add(new Product { Id = productId, Name = "To Delete", Description = "", ImageUrl = "", ProductSizes = new List<ProductSize>() });
            await _context.SaveChangesAsync();

            // Act
            var result = await _productService.DeleteProductAsync(productId);

            // Assert
            Assert.That(result, Is.True);
            Assert.That(await _context.Products.CountAsync(), Is.EqualTo(0));
        }

        [Test]
        public async Task DeleteProductAsync_InvalidId_ReturnsFalse()
        {
            // Act
            var result = await _productService.DeleteProductAsync(Guid.NewGuid());

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public async Task GetAllProductsAsync_ReturnsAllProductsMapped()
        {
            // Arrange
            var cat = new Category { Id = 1, Name = "Cat1" };
            var p1 = new Product { Id = Guid.NewGuid(), Name = "P1", Description = "", ImageUrl = "", Category = cat, ProductSizes = new List<ProductSize>() };
            var p2 = new Product { Id = Guid.NewGuid(), Name = "P2", Description = "", ImageUrl = "", Category = cat, ProductSizes = new List<ProductSize> { new() { Size = "S", Quantity = 5 } } };
            
            _context.Categories.Add(cat);
            _context.Products.AddRange(p1, p2);
            await _context.SaveChangesAsync();

            // Act
            var result = (await _productService.GetAllProductsAsync()).ToList();

            // Assert
            Assert.That(result.Count, Is.EqualTo(2));
            Assert.That(result.First(p => p.Name == "P1").CategoryName, Is.EqualTo("Cat1"));
            Assert.That(result.First(p => p.Name == "P2").AvailableSizes.Count, Is.EqualTo(1));
        }

        [Test]
        public async Task GetProductDetailsAsync_ValidId_ReturnsMappedDetails()
        {
            // Arrange
            var cat = new Category { Id = 1, Name = "Cat1" };
            var pId = Guid.NewGuid();
            var p1 = new Product { Id = pId, Name = "Detailed Product", Description = "", ImageUrl = "", Category = cat, ProductSizes = new List<ProductSize> { new() { Size = "L", Quantity = 3 } } };
            
            _context.Categories.Add(cat);
            _context.Products.Add(p1);
            await _context.SaveChangesAsync();

            // Act
            var result = await _productService.GetProductDetailsAsync(pId);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Name, Is.EqualTo("Detailed Product"));
            Assert.That(result.CategoryName, Is.EqualTo("Cat1"));
            Assert.That(result.AvailableSizes.Count, Is.EqualTo(1));
        }

        [Test]
        public async Task GetProductDetailsAsync_InvalidId_ReturnsNull()
        {
            // Act
            var result = await _productService.GetProductDetailsAsync(Guid.NewGuid());

            // Assert
            Assert.That(result, Is.Null);
        }
    }
}
