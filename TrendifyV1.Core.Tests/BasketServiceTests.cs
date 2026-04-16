using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System;
using System.Linq;
using System.Threading.Tasks;
using TrendifyV1.Core.Implementations;
using TrendifyV1.Data;
using TrendifyV1.Data.Entities;

namespace TrendifyV1.Core.Tests
{
    [TestFixture]
    public class BasketServiceTests
    {
        private TrendifyV1DbContext _context;
        private BasketService _basketService;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<TrendifyV1DbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Unique DB per test
                .Options;

            _context = new TrendifyV1DbContext(options);
            _basketService = new BasketService(_context);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Test]
        public async Task GetBasketAsync_ReturnsCorrectItems()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var product = new Product { Id = Guid.NewGuid(), Name = "Test Product", Description = "", Price = 10, ImageUrl = "test.jpg", CategoryId = 1 };
            var productSize = new ProductSize { Id = 1, Size = "M", Quantity = 10, Product = product };
            var basketItem = new BasketItem { Id = 1, UserId = userId, ProductSizeId = 1, Quantity = 2, ProductSize = productSize };

            _context.Products.Add(product);
            _context.ProductSizes.Add(productSize);
            _context.BasketItems.Add(basketItem);
            await _context.SaveChangesAsync();

            // Act
            var result = await _basketService.GetBasketAsync(userId);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Items.Count(), Is.EqualTo(1));
            var item = result.Items.First();
            Assert.That(item.ProductName, Is.EqualTo("Test Product"));
            Assert.That(item.Size, Is.EqualTo("M"));
            Assert.That(item.Price, Is.EqualTo(10));
            Assert.That(item.Quantity, Is.EqualTo(2));
        }

        [Test]
        public async Task AddToBasketAsync_NewItem_AddsToDatabase()
        {
            // Arrange
            var userId = Guid.NewGuid();
            
            // Act
            await _basketService.AddToBasketAsync(userId, 1, 2);

            // Assert
            var items = await _context.BasketItems.ToListAsync();
            Assert.That(items.Count, Is.EqualTo(1));
            Assert.That(items[0].UserId, Is.EqualTo(userId));
            Assert.That(items[0].ProductSizeId, Is.EqualTo(1));
            Assert.That(items[0].Quantity, Is.EqualTo(2));
        }

        [Test]
        public async Task AddToBasketAsync_ExistingItem_IncreasesQuantity()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var existingItem = new BasketItem { UserId = userId, ProductSizeId = 1, Quantity = 2 };
            _context.BasketItems.Add(existingItem);
            await _context.SaveChangesAsync();

            // Act
            await _basketService.AddToBasketAsync(userId, 1, 3);

            // Assert
            var items = await _context.BasketItems.ToListAsync();
            Assert.That(items.Count, Is.EqualTo(1));
            Assert.That(items[0].Quantity, Is.EqualTo(5));
        }

        [Test]
        public async Task RemoveFromBasketAsync_ExistingItem_RemovesFromDatabase()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var item = new BasketItem { Id = 1, UserId = userId, ProductSizeId = 1, Quantity = 2 };
            _context.BasketItems.Add(item);
            await _context.SaveChangesAsync();

            // Act
            await _basketService.RemoveFromBasketAsync(1, userId);

            // Assert
            var items = await _context.BasketItems.ToListAsync();
            Assert.That(items, Is.Empty);
        }

        [Test]
        public async Task RemoveFromBasketAsync_NonExistingItem_DoesNothing()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var item = new BasketItem { Id = 1, UserId = userId, ProductSizeId = 1, Quantity = 2 };
            _context.BasketItems.Add(item);
            await _context.SaveChangesAsync();

            // Act
            await _basketService.RemoveFromBasketAsync(2, userId); // Wrong ID

            // Assert
            var items = await _context.BasketItems.ToListAsync();
            Assert.That(items.Count, Is.EqualTo(1));
        }

        [Test]
        public async Task UpdateQuantityAsync_IncreasesQuantity()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var item = new BasketItem { Id = 1, UserId = userId, ProductSizeId = 1, Quantity = 2 };
            _context.BasketItems.Add(item);
            await _context.SaveChangesAsync();

            // Act
            await _basketService.UpdateQuantityAsync(1, userId, 3); // delta = +3

            // Assert
            var updatedItem = await _context.BasketItems.FindAsync(1);
            Assert.That(updatedItem.Quantity, Is.EqualTo(5));
        }

        [Test]
        public async Task UpdateQuantityAsync_DecreasesQuantity_NotRemoved()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var item = new BasketItem { Id = 1, UserId = userId, ProductSizeId = 1, Quantity = 5 };
            _context.BasketItems.Add(item);
            await _context.SaveChangesAsync();

            // Act
            await _basketService.UpdateQuantityAsync(1, userId, -2); // delta = -2

            // Assert
            var updatedItem = await _context.BasketItems.FindAsync(1);
            Assert.That(updatedItem.Quantity, Is.EqualTo(3));
        }

        [Test]
        public async Task UpdateQuantityAsync_DecreasesQuantityToZeroOrLess_RemovesItem()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var item = new BasketItem { Id = 1, UserId = userId, ProductSizeId = 1, Quantity = 2 };
            _context.BasketItems.Add(item);
            await _context.SaveChangesAsync();

            // Act
            await _basketService.UpdateQuantityAsync(1, userId, -2); // delta = -2

            // Assert
            var items = await _context.BasketItems.ToListAsync();
            Assert.That(items, Is.Empty);
        }

        [Test]
        public async Task ClearBasketAsync_RemovesAllUserItems()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var otherUserId = Guid.NewGuid();
            _context.BasketItems.AddRange(
                new BasketItem { Id = 1, UserId = userId, ProductSizeId = 1, Quantity = 1 },
                new BasketItem { Id = 2, UserId = userId, ProductSizeId = 2, Quantity = 2 },
                new BasketItem { Id = 3, UserId = otherUserId, ProductSizeId = 1, Quantity = 1 }
            );
            await _context.SaveChangesAsync();

            // Act
            await _basketService.ClearBasketAsync(userId);

            // Assert
            var items = await _context.BasketItems.ToListAsync();
            Assert.That(items.Count, Is.EqualTo(1));
            Assert.That(items[0].UserId, Is.EqualTo(otherUserId));
        }
    }
}
