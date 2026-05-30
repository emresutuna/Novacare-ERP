using Microsoft.AspNetCore.Mvc;
using NovacareERP.Application.Customers;
using NovacareERP.Domain.Enums;

namespace NovacareERP.Web.Controllers;

public sealed class CustomersController : Controller
{
    private readonly ICustomerDirectoryService _customerDirectoryService;

    public CustomersController(ICustomerDirectoryService customerDirectoryService)
    {
        _customerDirectoryService = customerDirectoryService;
    }

    public IActionResult Index(CustomerLegalType? legalType, string? searchTerm)
    {
        var model = _customerDirectoryService.GetList(legalType, searchTerm);
        return View(model);
    }

    public IActionResult Details(Guid id)
    {
        var model = _customerDirectoryService.GetById(id);

        if (model is null)
        {
            return NotFound();
        }

        return View(model);
    }

    public IActionResult Create()
    {
        var model = _customerDirectoryService.GetList(null, null);
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(CustomerFormViewModel form)
    {
        _customerDirectoryService.Add(form);
        TempData["CustomerSaved"] = "Musteri taslak listeye eklendi.";

        return RedirectToAction(nameof(Index), new { legalType = form.LegalType });
    }
}
