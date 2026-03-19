using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrendifyV1.Data.Entities;
using TrendifyV1.ViewModels.Category;

namespace TrendifyV1.Controllers
{
    public class CategoryController : Controller
    {
        private readonly TrendifyV1DbContext _context;

        public CategoryController(TrendifyV1DbContext context)
        {
            this._context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var categories = await _context.Categories
                .Select(c => new CategoryListViewModel
                {
                    Id = c.Id,
                    Name = c.Name,
                    ProductsCount = c.Products.Count
                })
                .ToListAsync();

            return View(categories);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View("Form", new CategoryFormViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Create(CategoryFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("Form", model);
            }

            var category = new Category
            {
                Name = model.Name
            };

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var category = await _context.Categories.FindAsync(id);

            if (category == null)
                return NotFound();

            var vm = new CategoryFormViewModel
            {
                Id = category.Id,
                Name = category.Name
            };

            return View("Form", vm);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(CategoryFormViewModel model)
        {
            if (!ModelState.IsValid)
                return View("Form", model);

            var category = await _context.Categories.FindAsync(model.Id);

            if (category == null)
                return NotFound();

            category.Name = model.Name;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var category = await _context.Categories
                .Include(c => c.Products)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (category == null)
                return NotFound();

            if (category.Products.Any())
            {
                TempData["error"] = "Cannot delete category with existing products!";
                return RedirectToAction(nameof(Index));
            }

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
