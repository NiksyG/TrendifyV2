using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrendifyV1.Core.Interfaces;
using TrendifyV1.Data.Entities;

namespace TrendifyV1.Core.Implementations
{
    public class OrderService(
        TrendifyV1DbContext context)
        : IOrderService
    {
        public async Task<bool> CreateOrderAsync(Guid userId)
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
    }
}
