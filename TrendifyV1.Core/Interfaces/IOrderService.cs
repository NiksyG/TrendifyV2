using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TrendifyV1.ViewModels.CheckoutViewModel;
using TrendifyV1.ViewModels.OrderAdminViewModels;
using TrendifyV1.ViewModels.UserOrderViewModels;

namespace TrendifyV1.Core.Interfaces
{
    public interface IOrderService
    {
        Task<bool> CreateOrderAsync(Guid userId, CheckoutViewModel model);
        Task<IEnumerable<OrderListAdminViewModel>> GetAllOrdersAsync();
        Task<OrderDetailsAdminViewModel?> GetOrderDetailsAsync(int orderId);
        Task<bool> UpdateOrderStatusAsync(int orderId, string newStatus);
        Task<IEnumerable<MyOrderViewModel>> GetUserOrdersAsync(Guid userId);
        Task<MyOrderDetailsViewModel?> GetUserOrderDetailsAsync(int orderId, Guid userId);
    }
}