using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using TrendifyV1.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using TrendifyV1.ViewModels.AccountViewModels;

namespace TrendifyV1.Controllers
{
    public class PasswordController(UserManager<ApplicationUser> userManager) : Controller
    {
        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ResetPasswordFastViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await userManager.FindByEmailAsync(model.Email);

            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Неуспешен опит за смяна на паролата. Проверете данните.");
                return View(model);
            }

            var token = await userManager.GeneratePasswordResetTokenAsync(user);

            var result = await userManager.ResetPasswordAsync(user, token, model.NewPassword);

            if (result.Succeeded)
            {
                TempData["Success"] = "Паролата беше успешно сменена! Вече можеш да влезеш с новата парола.";
                return RedirectToPage("/Account/Login", new { area = "Identity" });
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);
        }
    }
}