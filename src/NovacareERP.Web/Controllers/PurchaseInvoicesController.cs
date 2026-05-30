using Microsoft.AspNetCore.Mvc;
using NovacareERP.Application.Integrations;
using NovacareERP.Application.PurchaseInvoices;
using NovacareERP.Domain.Enums;

namespace NovacareERP.Web.Controllers;

public sealed class PurchaseInvoicesController : Controller
{
    private readonly IPurchaseInvoiceDirectoryService _purchaseInvoiceDirectoryService;
    private readonly IElectronicDocumentIntegration _electronicDocumentIntegration;

    public PurchaseInvoicesController(
        IPurchaseInvoiceDirectoryService purchaseInvoiceDirectoryService,
        IElectronicDocumentIntegration electronicDocumentIntegration)
    {
        _purchaseInvoiceDirectoryService = purchaseInvoiceDirectoryService;
        _electronicDocumentIntegration = electronicDocumentIntegration;
    }

    public IActionResult Index(string? searchTerm, InvoiceStatus? status, ElectronicDocumentType? documentType)
    {
        var model = _purchaseInvoiceDirectoryService.GetList(searchTerm, status, documentType);
        return View(model);
    }

    public IActionResult Details(Guid id)
    {
        var model = _purchaseInvoiceDirectoryService.GetById(id);

        if (model is null)
        {
            return NotFound();
        }

        return View(model);
    }

    public IActionResult Create(Guid? supplierId, ElectronicDocumentType? documentType)
    {
        var model = _purchaseInvoiceDirectoryService.GetDraft(supplierId, documentType);
        return View(model);
    }

    public IActionResult Edit(Guid id)
    {
        var model = _purchaseInvoiceDirectoryService.GetById(id);

        if (model is null)
        {
            return NotFound();
        }

        return View("Create", model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(PurchaseInvoiceFormViewModel form)
    {
        var id = _purchaseInvoiceDirectoryService.Add(form);
        TempData["InvoiceSaved"] = "Alis faturasi kaydedildi.";
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(Guid id, PurchaseInvoiceFormViewModel form)
    {
        var updated = _purchaseInvoiceDirectoryService.Update(id, form);

        if (!updated)
        {
            return NotFound();
        }

        TempData["InvoiceSaved"] = "Alis faturasi guncellendi.";
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SendElectronicDocument(Guid id, CancellationToken cancellationToken)
    {
        var invoice = _purchaseInvoiceDirectoryService.GetById(id);

        if (invoice is null)
        {
            return NotFound();
        }

        if (invoice.DocumentType == ElectronicDocumentType.None)
        {
            TempData["InvoiceSaved"] = "Manuel faturalar icin e-belge gonderimi yapilmaz.";
            return RedirectToAction(nameof(Details), new { id });
        }

        var gross = invoice.Items.Sum(item => item.TotalAmount + item.TotalAmount * item.TaxRate / 100m);
        var result = await _electronicDocumentIntegration.SendAsync(
            new ElectronicDocumentRequest(
                id,
                InvoiceType.Purchase,
                invoice.DocumentType,
                invoice.InvoiceNumber,
                invoice.SupplierName,
                invoice.SupplierTaxNumber,
                gross,
                invoice.CurrencyCode),
            cancellationToken);

        if (result.Succeeded && result.DocumentUuid is not null && result.Ettn is not null)
        {
            _purchaseInvoiceDirectoryService.ApplyElectronicDocumentResult(
                id,
                result.DocumentUuid,
                result.Ettn,
                ElectronicDocumentStatus.Pending);
        }

        TempData["InvoiceSaved"] = result.Message;
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> QueryElectronicDocumentStatus(Guid id, CancellationToken cancellationToken)
    {
        var invoice = _purchaseInvoiceDirectoryService.GetById(id);

        if (invoice is null || string.IsNullOrWhiteSpace(invoice.DocumentUuid))
        {
            TempData["InvoiceSaved"] = "Sorgulanacak e-belge bulunamadi.";
            return RedirectToAction(nameof(Details), new { id });
        }

        var result = await _electronicDocumentIntegration.GetStatusAsync(invoice.DocumentUuid, cancellationToken);
        _purchaseInvoiceDirectoryService.ApplyElectronicDocumentResult(id, result.DocumentUuid, result.Ettn ?? invoice.Ettn ?? "", result.Status);
        TempData["InvoiceSaved"] = result.Message;
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CancelElectronicDocument(Guid id, CancellationToken cancellationToken)
    {
        var invoice = _purchaseInvoiceDirectoryService.GetById(id);

        if (invoice is null || string.IsNullOrWhiteSpace(invoice.DocumentUuid))
        {
            TempData["InvoiceSaved"] = "Iptal edilecek e-belge bulunamadi.";
            return RedirectToAction(nameof(Details), new { id });
        }

        var result = await _electronicDocumentIntegration.CancelAsync(invoice.DocumentUuid, cancellationToken);

        if (result.Succeeded)
        {
            _purchaseInvoiceDirectoryService.ApplyElectronicDocumentResult(
                id,
                invoice.DocumentUuid,
                invoice.Ettn ?? "",
                ElectronicDocumentStatus.Cancelled);
        }

        TempData["InvoiceSaved"] = result.Message;
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Delete(Guid id)
    {
        var deleted = _purchaseInvoiceDirectoryService.Delete(id);

        if (!deleted)
        {
            return NotFound();
        }

        TempData["InvoiceSaved"] = "Alis faturasi silindi.";
        return RedirectToAction(nameof(Index));
    }
}
