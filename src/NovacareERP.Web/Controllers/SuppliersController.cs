using Microsoft.AspNetCore.Mvc;
using NovacareERP.Application.Suppliers;
using NovacareERP.Domain.Enums;

namespace NovacareERP.Web.Controllers;

public sealed class SuppliersController : Controller
{
    private readonly ISupplierDirectoryService _supplierDirectoryService;

    public SuppliersController(ISupplierDirectoryService supplierDirectoryService)
    {
        _supplierDirectoryService = supplierDirectoryService;
    }

    public IActionResult Index(CustomerLegalType? legalType, string? searchTerm)
    {
        var model = _supplierDirectoryService.GetList(legalType, searchTerm);
        return View(model);
    }

    public IActionResult Details(Guid id)
    {
        var model = _supplierDirectoryService.GetById(id);

        if (model is null)
        {
            return NotFound();
        }

        return View(model);
    }

    public IActionResult Create()
    {
        var model = _supplierDirectoryService.GetList(null, null);
        return View(model);
    }

    public IActionResult Edit(Guid id)
    {
        var supplier = _supplierDirectoryService.GetById(id);

        if (supplier is null)
        {
            return NotFound();
        }

        var options = _supplierDirectoryService.GetList(null, null).LegalTypeOptions;

        return View(new SupplierListViewModel
        {
            LegalTypeOptions = options,
            Form = new SupplierFormViewModel
            {
                Id = supplier.Id,
                Title = "Tedarikci Duzenle",
                DisplayName = supplier.DisplayName,
                LegalType = supplier.LegalType,
                TaxNumber = supplier.TaxNumber,
                TaxOffice = supplier.TaxOffice,
                ContactPerson = supplier.ContactPerson,
                ContactTitle = supplier.ContactTitle,
                Email = supplier.Email,
                Phone = supplier.Phone,
                City = supplier.City,
                CurrencyCode = supplier.CurrencyCode,
                PurchaseLimit = supplier.PurchaseLimit
            }
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(SupplierFormViewModel form)
    {
        _supplierDirectoryService.Add(form);
        TempData["SupplierSaved"] = "Tedarikci taslak listeye eklendi.";

        return RedirectToAction(nameof(Index), new { legalType = form.LegalType });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(Guid id, SupplierFormViewModel form)
    {
        var updated = _supplierDirectoryService.Update(id, form);

        if (!updated)
        {
            return NotFound();
        }

        TempData["SupplierSaved"] = "Tedarikci bilgileri guncellendi.";
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Delete(Guid id)
    {
        var deleted = _supplierDirectoryService.Delete(id);

        if (!deleted)
        {
            return NotFound();
        }

        TempData["SupplierSaved"] = "Tedarikci kaydi silindi.";
        return RedirectToAction(nameof(Index));
    }
}
