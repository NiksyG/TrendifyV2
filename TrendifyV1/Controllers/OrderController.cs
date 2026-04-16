using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using TrendifyV1.Core.Interfaces;
using TrendifyV1.ViewModels.CheckoutViewModel;
using TrendifyV1.ViewModels.OrderAdminViewModels;
using TrendifyV1.ViewModels.UserOrderViewModels;

namespace TrendifyV1.Controllers
{
    [Authorize]
    public class OrderController(
        IOrderService orderService,
        IBasketService basketService)
        : Controller
    {
        private Guid GetCurrentUserId()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return Guid.Parse(userIdString);
        }

        [HttpGet]
        public async Task<IActionResult> Checkout()
        {
            var userId = GetCurrentUserId();
            var basket = await basketService.GetBasketAsync(userId);

            if (basket == null || basket.Items.Count == 0)
            {
                return RedirectToAction("Index", "Home");
            }

            var model = new CheckoutViewModel
            {
                TotalAmount = basket.TotalAmount
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Checkout(CheckoutViewModel model)
        {
            var userId = GetCurrentUserId();

            if (!ModelState.IsValid)
            {
                var basket = await basketService.GetBasketAsync(userId);
                model.TotalAmount = basket?.TotalAmount ?? 0;
                return View(model);
            }

            var success = await orderService.CreateOrderAsync(userId, model);

            if (!success)
            {
                TempData["Error"] = "Възникна грешка при обработката на поръчката!";
                return RedirectToAction(nameof(Checkout));
            }

            return RedirectToAction(nameof(Success));
        }

        [HttpGet]
        public IActionResult Success()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> MyOrders()
        {
            var userId = GetCurrentUserId();
            var orders = await orderService.GetUserOrdersAsync(userId);
            return View(orders);
        }

        [HttpGet]
        public async Task<IActionResult> MyOrderDetails(int id)
        {
            var userId = GetCurrentUserId();
            var order = await orderService.GetUserOrderDetailsAsync(id, userId);

            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        [Authorize(Roles = "Administrator")]
        [HttpGet]
        public async Task<IActionResult> AdminIndex()
        {
            var orders = await orderService.GetAllOrdersAsync();
            return View(orders);
        }

        [Authorize(Roles = "Administrator")]
        [HttpGet]
        public async Task<IActionResult> AdminDetails(int id)
        {
            var order = await orderService.GetOrderDetailsAsync(id);

            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        [Authorize(Roles = "Administrator")]
        [HttpPost]
        public async Task<IActionResult> UpdateStatus(int id, string status)
        {
            var success = await orderService.UpdateOrderStatusAsync(id, status);

            if (success)
            {
                TempData["Success"] = "Статусът на поръчката беше обновен!";
            }
            else
            {
                TempData["Error"] = "Възникна грешка при обновяване на статуса.";
            }

            return RedirectToAction(nameof(AdminDetails), new { id = id });
        }
    }
}