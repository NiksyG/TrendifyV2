using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrendifyV1.ViewModels.BasketViewModels;

namespace TrendifyV1.Core.Interfaces
{
    public interface IBasketService
    {
        Task<BasketViewModel> GetBasketAsync(Guid userId);
        Task AddToBasketAsync(Guid userId, int productSizeId, int quantity);
        Task RemoveFromBasketAsync(int basketItemId, Guid userId);
        Task ClearBasketAsync(Guid userId);
    }
}
