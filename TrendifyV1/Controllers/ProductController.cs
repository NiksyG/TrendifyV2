using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrendifyV1.Data;
using TrendifyV1.Data.Entities;
using TrendifyV1.ViewModels.Product;

public class ProductController : Controller
{
    private readonly TrendifyV1DbContext _context;

    public ProductController(TrendifyV1DbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var products = await _context.Products
            .Include(p => p.Category)
            .Select(p => new ProductListViewModel
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price,
                ImageUrl = p.ImageUrl,
                CategoryName = p.Category.Name
            })
            .ToListAsync();

        return View(products);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var vm = new ProductFormViewModel
        {
            Categories = await _context.Categories.ToListAsync(),
            Sizes = new List<ProductSizeInputModel>
        {
            new() { Size = "S" },
            new() { Size = "M" },
            new() { Size = "L" }
        }
        };

        return View("Form", vm);
    }

    [HttpPost]
    public async Task<IActionResult> Create(ProductFormViewModel model)
    {
        if (!ModelState.IsValid)
        {
            model.Categories = await _context.Categories.ToListAsync();
            return View("Form", model);
        }

        var product = new Product
        {
            Id = Guid.NewGuid(),
            Name = model.Name,
            Description = model.Description,
            Price = model.Price,
            ImageUrl = model.ImageUrl,
            CategoryId = model.CategoryId,
            ProductSizes = model.Sizes.Select(s => new ProductSize
            {
                Size = s.Size,
                Quantity = s.Quantity
            }).ToList()
        };

        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(Guid id)
    {
        var product = await _context.Products
            .Include(p => p.ProductSizes)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (product == null)
            return NotFound();

        var vm = new ProductFormViewModel
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            ImageUrl = product.ImageUrl,
            CategoryId = product.CategoryId,
            Categories = await _context.Categories.ToListAsync(),

            Sizes = product.ProductSizes.Select(s => new ProductSizeInputModel
            {
                Id = s.Id,
                Size = s.Size,
                Quantity = s.Quantity
            }).ToList()
        };

        return View("Form", vm);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(ProductFormViewModel model)
    {
        var product = await _context.Products
            .Include(p => p.ProductSizes)
            .FirstOrDefaultAsync(p => p.Id == model.Id);

        if (product == null)
            return NotFound();

        product.Name = model.Name;
        product.Description = model.Description;
        product.Price = model.Price;
        product.ImageUrl = model.ImageUrl;
        product.CategoryId = model.CategoryId;

        product.ProductSizes.Clear();

        foreach (var s in model.Sizes)
        {
            product.ProductSizes.Add(new ProductSize
            {
                Size = s.Size,
                Quantity = s.Quantity
            });
        }

        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> Delete(Guid id)
    {
        var product = await _context.Products.FindAsync(id);

        if (product == null)
            return NotFound();

        _context.Products.Remove(product);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }
}