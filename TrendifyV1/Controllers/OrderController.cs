using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using TrendifyV1.Core.Interfaces;
namespace TrendifyV1.Controllers
{
    [Authorize]
    public class OrderController(
        IOrderService orderService)
        : Controller
    {
        private Guid GetCurrentUserId()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return Guid.Parse(userIdString);
        }

        [HttpPost]
        public async Task<IActionResult> Create()
        {
            var userId = GetCurrentUserId();
            var success = await orderService.CreateOrderAsync(userId);

            if (!success)
            {
                TempData["Error"] = "Количката ви е празна или възникна грешка!";
                return RedirectToAction("Index", "Basket");
            }

            TempData["Success"] = "Успешно направихте поръчка!";
            return RedirectToAction("Index", "Home");
        }
    }
}