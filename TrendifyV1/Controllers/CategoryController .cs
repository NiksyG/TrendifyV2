using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TrendifyV1.Core.Interfaces;
using TrendifyV1.ViewModels.CategoryViewModels;

namespace TrendifyV1.Controllers
{
    public class CategoryController(
        ICategoryService categoryService) 
        : Controller
    {

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var categories = await categoryService.GetAllCategoriesAsync();
            return View(categories);
        }

        [Authorize(Roles = "Administrator")]
        [HttpGet]
        public IActionResult Create()
        {
            return View("Form", new CategoryFormViewModel());
        }

        [Authorize(Roles = "Administrator")]
        [HttpPost]
        public async Task<IActionResult> Create(CategoryFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("Form", model);
            }

            await categoryService.CreateCategoryAsync(model);
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Administrator")]
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var vm = await categoryService.GetCategoryByIdAsync(id);

            if (vm == null)
                return NotFound();

            return View("Form", vm);
        }

        [Authorize(Roles = "Administrator")]
        [HttpPost]
        public async Task<IActionResult> Edit(CategoryFormViewModel model)
        {
            if (!ModelState.IsValid)
                return View("Form", model);

            await categoryService.UpdateCategoryAsync(model);
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Administrator")]
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await categoryService.DeleteCategoryAsync(id);

            if (!success)
            {
                TempData["error"] = "Cannot delete category with existing products or category not found!";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
