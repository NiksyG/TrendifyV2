using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrendifyV1.Core.Interfaces;
using TrendifyV1.Data.Entities;
using TrendifyV1.ViewModels.CheckoutViewModel;
using TrendifyV1.ViewModels.OrderAdminViewModels;
using TrendifyV1.ViewModels.UserOrderViewModels;

namespace TrendifyV1.Core.Implementations
{
    public class OrderService(TrendifyV1DbContext context) : IOrderService
    {
        public async Task<bool> CreateOrderAsync(Guid userId, CheckoutViewModel model)
        {
            var basketItems = await context.BasketItems
                .Include(b => b.ProductSize)
                .ThenInclude(ps => ps.Product)
                .Where(b => b.UserId == userId)
                .ToListAsync();

            if (!basketItems.Any()) return false;

            var order = new Order
            {
                UserId = userId,
                OrderDate = DateTime.UtcNow,
                TotalPrice = basketItems.Sum(i => i.Quantity * i.ProductSize.Product.Price),
                FullName = model.FullName,
                Phone = model.Phone,
                City = model.City,
                Address = model.Address,
                Status = "Обработва се",
                OrderDetails = basketItems.Select(b => new OrderDetail
                {
                    ProductSizeId = b.ProductSizeId,
                    Quantity = b.Quantity,
                    Price = b.ProductSize.Product.Price
                }).ToList()
            };

            context.Orders.Add(order);
            context.BasketItems.RemoveRange(basketItems);

            await context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<OrderListAdminViewModel>> GetAllOrdersAsync()
        {
            return await context.Orders
                .OrderByDescending(o => o.OrderDate)
                .Select(o => new OrderListAdminViewModel
                {
                    Id = o.Id,
                    OrderDate = o.OrderDate,
                    CustomerName = o.FullName,
                    TotalPrice = o.TotalPrice,
                    Status = o.Status
                })
                .ToListAsync();
        }

        public async Task<OrderDetailsAdminViewModel?> GetOrderDetailsAsync(int orderId)
        {
            return await context.Orders
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.ProductSize)
                .ThenInclude(ps => ps.Product)
                .Where(o => o.Id == orderId)
                .Select(o => new OrderDetailsAdminViewModel
                {
                    Id = o.Id,
                    OrderDate = o.OrderDate,
                    Status = o.Status,
                    TotalAmount = o.TotalPrice,
                    FullName = o.FullName,
                    Phone = o.Phone,
                    City = o.City,
                    Address = o.Address,
                    Items = o.OrderDetails.Select(oi => new OrderItemAdminViewModel
                    {
                        ProductName = oi.ProductSize.Product.Name,
                        Size = oi.ProductSize.Size,
                        Quantity = oi.Quantity,
                        Price = oi.Price
                    }).ToList()
                })
                .FirstOrDefaultAsync();
        }

        public async Task<bool> UpdateOrderStatusAsync(int orderId, string newStatus)
        {
            var order = await context.Orders.FindAsync(orderId);

            if (order == null) return false;

            order.Status = newStatus;
            await context.SaveChangesAsync();

            return true;
        }

        public async Task<IEnumerable<MyOrderViewModel>> GetUserOrdersAsync(Guid userId)
        {
            return await context.Orders
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.OrderDate)
                .Select(o => new MyOrderViewModel
                {
                    Id = o.Id,
                    OrderDate = o.OrderDate,
                    TotalPrice = o.TotalPrice,
                    Status = o.Status
                })
                .ToListAsync();
        }

        public async Task<MyOrderDetailsViewModel?> GetUserOrderDetailsAsync(int orderId, Guid userId)
        {
            return await context.Orders
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.ProductSize)
                .ThenInclude(ps => ps.Product)
                .Where(o => o.Id == orderId && o.UserId == userId)
                .Select(o => new MyOrderDetailsViewModel
                {
                    Id = o.Id,
                    OrderDate = o.OrderDate,
                    Status = o.Status,
                    TotalAmount = o.TotalPrice,
                    FullName = o.FullName,
                    Phone = o.Phone,
                    City = o.City,
                    Address = o.Address,
                    Items = o.OrderDetails.Select(oi => new MyOrderItemViewModel
                    {
                        ProductName = oi.ProductSize.Product.Name,
                        Size = oi.ProductSize.Size,
                        Quantity = oi.Quantity,
                        Price = oi.Price
                    }).ToList()
                })
                .FirstOrDefaultAsync();
        }
    }
}