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

    public IActionResult Edit(Guid id)
    {
        var customer = _customerDirectoryService.GetById(id);

        if (customer is null)
        {
            return NotFound();
        }

        var options = _customerDirectoryService.GetList(null, null).LegalTypeOptions;

        return View(new CustomerListViewModel
        {
            LegalTypeOptions = options,
            Form = new CustomerFormViewModel
            {
                Id = customer.Id,
                Title = "Musteri Duzenle",
                DisplayName = customer.DisplayName,
                LegalType = customer.LegalType,
                TaxNumber = customer.TaxNumber,
                TaxOffice = customer.TaxOffice,
                ContactPerson = customer.ContactPerson,
                ContactTitle = customer.ContactTitle,
                Email = customer.Email,
                Phone = customer.Phone,
                City = customer.City,
                CurrencyCode = customer.CurrencyCode,
                CreditLimit = customer.CreditLimit
            }
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(CustomerFormViewModel form)
    {
        _customerDirectoryService.Add(form);
        TempData["CustomerSaved"] = "Musteri taslak listeye eklendi.";

        return RedirectToAction(nameof(Index), new { legalType = form.LegalType });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(Guid id, CustomerFormViewModel form)
    {
        var updated = _customerDirectoryService.Update(id, form);

        if (!updated)
        {
            return NotFound();
        }

        TempData["CustomerSaved"] = "Musteri bilgileri guncellendi.";
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Delete(Guid id)
    {
        var deleted = _customerDirectoryService.Delete(id);

        if (!deleted)
        {
            return NotFound();
        }

        TempData["CustomerSaved"] = "Musteri kaydi silindi.";
        return RedirectToAction(nameof(Index));
    }
}
