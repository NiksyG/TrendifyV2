using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrendifyV1.Core.Interfaces;
using TrendifyV1.Data.Entities;
using TrendifyV1.ViewModels.BasketViewModels;

namespace TrendifyV1.Core.Implementations
{
    public class BasketService(
        TrendifyV1DbContext context)
        : IBasketService
    {
        public async Task<BasketViewModel> GetBasketAsync(Guid userId)
        {
            var basketItems = await context.BasketItems
                .Include(b => b.ProductSize)
                .ThenInclude(ps => ps.Product)
                .Where(b => b.UserId == userId)
                .Select(b => new BasketItemViewModel
                {
                    Id = b.Id,
                    ProductName = b.ProductSize.Product.Name,
                    Size = b.ProductSize.Size,
                    Price = b.ProductSize.Product.Price,
                    Quantity = b.Quantity,
                    ImageUrl = b.ProductSize.Product.ImageUrl
                })
                .ToListAsync();

            return new BasketViewModel
            {
                Items = basketItems
            };
        }

        public async Task AddToBasketAsync(Guid userId, int productSizeId, int quantity)
        {
            var existingItem = await context.BasketItems
                .FirstOrDefaultAsync(b => b.UserId == userId && b.ProductSizeId == productSizeId);

            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
            }
            else
            {
                var newItem = new BasketItem
                {
                    UserId = userId,
                    ProductSizeId = productSizeId,
                    Quantity = quantity
                };
                context.BasketItems.Add(newItem);
            }

            await context.SaveChangesAsync();
        }

        public async Task RemoveFromBasketAsync(int basketItemId, Guid userId)
        {
            var item = await context.BasketItems
                .FirstOrDefaultAsync(b => b.Id == basketItemId && b.UserId == userId);

            if (item != null)
            {
                context.BasketItems.Remove(item);
                await context.SaveChangesAsync();
            }
        }

        public async Task UpdateQuantityAsync(int basketItemId, Guid userId, int delta)
        {
            var item = await context.BasketItems
                .FirstOrDefaultAsync(b => b.Id == basketItemId && b.UserId == userId);

            if (item != null)
            {
                item.Quantity += delta;

                if (item.Quantity <= 0)
                {
                    context.BasketItems.Remove(item);
                }

                await context.SaveChangesAsync();
            }
        }

        public async Task ClearBasketAsync(Guid userId)
        {
            var items = await context.BasketItems.Where(b => b.UserId == userId).ToListAsync();
            context.BasketItems.RemoveRange(items);
            await context.SaveChangesAsync();
        }
    }
}