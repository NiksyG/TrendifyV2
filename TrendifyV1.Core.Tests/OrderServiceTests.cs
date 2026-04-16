using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrendifyV1.Core.Implementations;
using TrendifyV1.Data;
using TrendifyV1.Data.Entities;
using TrendifyV1.ViewModels.CheckoutViewModel;

namespace TrendifyV1.Core.Tests
{
    [TestFixture]
    public class OrderServiceTests
    {
        private TrendifyV1DbContext _context;
        private OrderService _orderService;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<TrendifyV1DbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new TrendifyV1DbContext(options);
            _orderService = new OrderService(_context);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Test]
        public async Task CreateOrderAsync_NoBasketItems_ReturnsFalse()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var model = new CheckoutViewModel();

            // Act
            var result = await _orderService.CreateOrderAsync(userId, model);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public async Task CreateOrderAsync_WithItems_CreatesOrderAndClearsBasket()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var product = new Product { Id = Guid.NewGuid(), Name = "Shoes", Price = 100, ImageUrl = "", Description = "", CategoryId = 1 };
            var productSize = new ProductSize { Id = 1, Size = "42", Quantity = 10, Product = product };
            var basketItem = new BasketItem { Id = 1, UserId = userId, ProductSizeId = 1, Quantity = 2, ProductSize = productSize };
            
            _context.Products.Add(product);
            _context.ProductSizes.Add(productSize);
            _context.BasketItems.Add(basketItem);
            await _context.SaveChangesAsync();

            var checkoutModel = new CheckoutViewModel 
            { 
                FullName = "John Doe", 
                Phone = "12345", 
                City = "NY", 
                Address = "5th Ave" 
            };

            // Act
            var result = await _orderService.CreateOrderAsync(userId, checkoutModel);

            // Assert
            Assert.That(result, Is.True);
            
            var order = await _context.Orders.Include(o => o.OrderDetails).FirstOrDefaultAsync();
            Assert.That(order, Is.Not.Null);
            Assert.That(order.UserId, Is.EqualTo(userId));
            Assert.That(order.TotalPrice, Is.EqualTo(200)); // 2 items * $100
            Assert.That(order.FullName, Is.EqualTo("John Doe"));
            Assert.That(order.Status, Is.EqualTo("Обработва се"));
            
            Assert.That(order.OrderDetails.Count, Is.EqualTo(1));
            Assert.That(order.OrderDetails.First().Quantity, Is.EqualTo(2));

            // Basket must be empty
            var remainingBasket = await _context.BasketItems.ToListAsync();
            Assert.That(remainingBasket, Is.Empty);
        }

        [Test]
        public async Task GetAllOrdersAsync_ReturnsAdminOrderViewModels()
        {
            // Arrange
            _context.Orders.AddRange(
                new Order { Id = 1, OrderDate = DateTime.Now.AddDays(-1), FullName = "A", Address="", City="", Phone="", TotalPrice = 10, Status = "Pending", OrderDetails = new List<OrderDetail>() },
                new Order { Id = 2, OrderDate = DateTime.Now, FullName = "B", Address="", City="", Phone="", TotalPrice = 20, Status = "Shipped", OrderDetails = new List<OrderDetail>() }
            );
            await _context.SaveChangesAsync();

            // Act
            var result = (await _orderService.GetAllOrdersAsync()).ToList();

            // Assert
            Assert.That(result.Count, Is.EqualTo(2));
            Assert.That(result[0].CustomerName, Is.EqualTo("B"), "Should sort descending by OrderDate");
            Assert.That(result[1].CustomerName, Is.EqualTo("A"));
        }

        [Test]
        public async Task GetOrderDetailsAsync_ValidId_ReturnsDetails()
        {
            // Arrange
            var product = new Product { Id = Guid.NewGuid(), Name = "Book", Price = 15, ImageUrl = "", Description = "", CategoryId = 1 };
            var productSize = new ProductSize { Id = 1, Size = "-", Quantity = 10, Product = product };
            
            var orderDetail = new OrderDetail { Id = 1, OrderId = 1, ProductSizeId = 1, Quantity = 2, Price = 15, ProductSize = productSize };
            var order = new Order { Id = 1, FullName = "Test Guy", Address="", City="", Phone="", OrderDetails = new List<OrderDetail> { orderDetail } };

            _context.Products.Add(product);
            _context.ProductSizes.Add(productSize);
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            // Act
            var result = await _orderService.GetOrderDetailsAsync(1);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.FullName, Is.EqualTo("Test Guy"));
            Assert.That(result.Items.Count, Is.EqualTo(1));
            Assert.That(result.Items.First().ProductName, Is.EqualTo("Book"));
            Assert.That(result.Items.First().Quantity, Is.EqualTo(2));
        }

        [Test]
        public async Task UpdateOrderStatusAsync_ValidId_UpdatesStatusReturnsTrue()
        {
            // Arrange
            _context.Orders.Add(new Order { Id = 1, Status = "OldStatus", FullName="", Address="", City="", Phone="", OrderDetails = new List<OrderDetail>() });
            await _context.SaveChangesAsync();

            // Act
            var result = await _orderService.UpdateOrderStatusAsync(1, "NewStatus");

            // Assert
            Assert.That(result, Is.True);
            var updatedOrder = await _context.Orders.FindAsync(1);
            Assert.That(updatedOrder.Status, Is.EqualTo("NewStatus"));
        }

        [Test]
        public async Task UpdateOrderStatusAsync_InvalidId_ReturnsFalse()
        {
            // Act
            var result = await _orderService.UpdateOrderStatusAsync(999, "NewStatus");

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public async Task GetUserOrdersAsync_ReturnsOnlyUserOrders()
        {
            // Arrange
            var user1 = Guid.NewGuid();
            var user2 = Guid.NewGuid();

            _context.Orders.AddRange(
                new Order { Id = 1, UserId = user1, OrderDate = DateTime.Now, FullName="", Address="", City="", Phone="", OrderDetails = new List<OrderDetail>() },
                new Order { Id = 2, UserId = user1, OrderDate = DateTime.Now.AddDays(-1), FullName="", Address="", City="", Phone="", OrderDetails = new List<OrderDetail>() },
                new Order { Id = 3, UserId = user2, OrderDate = DateTime.Now, FullName="", Address="", City="", Phone="", OrderDetails = new List<OrderDetail>() }
            );
            await _context.SaveChangesAsync();

            // Act
            var result = (await _orderService.GetUserOrdersAsync(user1)).ToList();

            // Assert
            Assert.That(result.Count, Is.EqualTo(2));
            Assert.That(result[0].Id, Is.EqualTo(1)); // Descending order
            Assert.That(result[1].Id, Is.EqualTo(2));
        }

        [Test]
        public async Task GetUserOrderDetailsAsync_ValidMatch_ReturnsDetails()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var product = new Product { Id = Guid.NewGuid(), Name = "User Book", Price = 15, ImageUrl = "", Description = "", CategoryId = 1 };
            var productSize = new ProductSize { Id = 1, Size = "-", Quantity = 10, Product = product };
            
            var orderDetail = new OrderDetail { Id = 1, OrderId = 1, ProductSizeId = 1, Quantity = 2, Price = 15, ProductSize = productSize };
            var order = new Order { Id = 1, UserId = userId, FullName = "User Order", Address="", City="", Phone="", OrderDetails = new List<OrderDetail> { orderDetail } };

            _context.Products.Add(product);
            _context.ProductSizes.Add(productSize);
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            // Act
            var result = await _orderService.GetUserOrderDetailsAsync(1, userId);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.EqualTo(1));
            Assert.That(result.FullName, Is.EqualTo("User Order"));
        }

        [Test]
        public async Task GetUserOrderDetailsAsync_WrongUserId_ReturnsNull()
        {
            // Arrange
            var order = new Order { Id = 1, UserId = Guid.NewGuid(), FullName="", Address="", City="", Phone="", OrderDetails = new List<OrderDetail>() };
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            // Act
            var result = await _orderService.GetUserOrderDetailsAsync(1, Guid.NewGuid()); // different ID

            // Assert
            Assert.That(result, Is.Null);
        }
    }
}
