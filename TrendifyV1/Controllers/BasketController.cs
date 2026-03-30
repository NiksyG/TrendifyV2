using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TrendifyV1.Core.Interfaces;

namespace TrendifyV1.Controllers
{
    [Authorize]
    public class BasketController(
        IBasketService basketService)
        : Controller
    {
        private Guid GetCurrentUserId()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return Guid.Parse(userIdString);
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var userId = GetCurrentUserId();
            var basketVm = await basketService.GetBasketAsync(userId);

            return View(basketVm);
        }

        [HttpPost]
        public async Task<IActionResult> Add(int productSizeId, int quantity = 1)
        {
            var userId = GetCurrentUserId();
            await basketService.AddToBasketAsync(userId, productSizeId, quantity);

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Remove(int id)
        {
            var userId = GetCurrentUserId();
            await basketService.RemoveFromBasketAsync(id, userId);

            return RedirectToAction(nameof(Index));
        }
        [HttpPost]
        public async Task<IActionResult> AddAjax(int productSizeId, int quantity = 1)
        {
            try
            {
                var userId = GetCurrentUserId();
                await basketService.AddToBasketAsync(userId, productSizeId, quantity);

                return Json(new { success = true });
            }
            catch
            {
                return Json(new { success = false, message = "Грешка при добавяне" });
            }
        }
    }
}
