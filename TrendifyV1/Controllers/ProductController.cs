using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrendifyV1.Core.Interfaces;
using TrendifyV1.Data.Entities;
using TrendifyV1.ViewModels.CategoryViewModels;
using TrendifyV1.ViewModels.ProductViewModels;

namespace TrendifyV1.Controllers;
public class ProductController(IProductService productService) : Controller
{
  

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var products = await productService.GetAllProductsAsync();
        return View(products);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var vm = await productService.GetProductForCreateAsync();
        return View("Form", vm);
    }

    [HttpPost]
    public async Task<IActionResult> Create(ProductFormViewModel model)
    {
        if (!ModelState.IsValid)
        {
            model.Categories = await productService.GetCategoriesForDropdownAsync();
            return View("Form", model);
        }

        await productService.CreateProductAsync(model);
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(Guid id)
    {
        var vm = await productService.GetProductForEditAsync(id);

        if (vm == null)
            return NotFound();

        return View("Form", vm);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(ProductFormViewModel model)
    {
        if (!ModelState.IsValid)
        {
            model.Categories = await productService.GetCategoriesForDropdownAsync();
            return View("Form", model);
        }

        var success = await productService.UpdateProductAsync(model);

        if (!success)
            return NotFound();

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> Delete(Guid id)
    {
        var success = await productService.DeleteProductAsync(id);

        if (!success)
            return NotFound();

        return RedirectToAction(nameof(Index));
    }
}
