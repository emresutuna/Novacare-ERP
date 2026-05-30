using Microsoft.AspNetCore.Mvc;
using NovacareERP.Application.Products;

namespace NovacareERP.Web.Controllers;

public sealed class ProductsController : Controller
{
    private readonly IProductDirectoryService _productDirectoryService;

    public ProductsController(IProductDirectoryService productDirectoryService)
    {
        _productDirectoryService = productDirectoryService;
    }

    public IActionResult Index(string? searchTerm, string? categoryName, string? brandName)
    {
        var model = _productDirectoryService.GetProducts(searchTerm, categoryName, brandName);
        return View(model);
    }

    public IActionResult Warehouses()
    {
        var model = _productDirectoryService.GetWorkspace();
        return View(model);
    }

    public IActionResult Production()
    {
        var model = _productDirectoryService.GetWorkspace();
        return View(model);
    }

    public IActionResult PriceLists()
    {
        var model = _productDirectoryService.GetWorkspace();
        return View(model);
    }

    public IActionResult Catalogs()
    {
        var model = _productDirectoryService.GetWorkspace();
        return View(model);
    }

    public IActionResult Variants()
    {
        var model = _productDirectoryService.GetWorkspace();
        return View(model);
    }
}
