using Microsoft.AspNetCore.Mvc;
using NovacareERP.Application.Integrations;
using NovacareERP.Application.SalesInvoices;
using NovacareERP.Domain.Enums;

namespace NovacareERP.Web.Controllers;

public sealed class SalesInvoicesController : Controller
{
    private readonly ISalesInvoiceDirectoryService _salesInvoiceDirectoryService;
    private readonly IElectronicDocumentIntegration _electronicDocumentIntegration;

    public SalesInvoicesController(
        ISalesInvoiceDirectoryService salesInvoiceDirectoryService,
        IElectronicDocumentIntegration electronicDocumentIntegration)
    {
        _salesInvoiceDirectoryService = salesInvoiceDirectoryService;
        _electronicDocumentIntegration = electronicDocumentIntegration;
    }

    public IActionResult Index(string? searchTerm, InvoiceStatus? status, ElectronicDocumentType? documentType)
    {
        var model = _salesInvoiceDirectoryService.GetList(searchTerm, status, documentType);
        return View(model);
    }

    public IActionResult Details(Guid id)
    {
        var model = _salesInvoiceDirectoryService.GetById(id);

        if (model is null)
        {
            return NotFound();
        }

        return View(model);
    }

    public IActionResult Create(Guid? customerId, ElectronicDocumentType? documentType)
    {
        var model = _salesInvoiceDirectoryService.GetDraft(customerId, documentType);
        return View(model);
    }

    public IActionResult Edit(Guid id)
    {
        var model = _salesInvoiceDirectoryService.GetById(id);

        if (model is null)
        {
            return NotFound();
        }

        return View("Create", model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(SalesInvoiceFormViewModel form)
    {
        var id = _salesInvoiceDirectoryService.Add(form);
        TempData["InvoiceSaved"] = "Satis faturasi kaydedildi.";
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(Guid id, SalesInvoiceFormViewModel form)
    {
        var updated = _salesInvoiceDirectoryService.Update(id, form);

        if (!updated)
        {
            return NotFound();
        }

        TempData["InvoiceSaved"] = "Satis faturasi guncellendi.";
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SendElectronicDocument(Guid id, CancellationToken cancellationToken)
    {
        var invoice = _salesInvoiceDirectoryService.GetById(id);

        if (invoice is null)
        {
            return NotFound();
        }

        var gross = invoice.Items.Sum(item => item.TotalAmount + item.TotalAmount * item.TaxRate / 100m);
        var result = await _electronicDocumentIntegration.SendAsync(
            new ElectronicDocumentRequest(
                id,
                InvoiceType.Sales,
                invoice.DocumentType,
                invoice.InvoiceNumber,
                invoice.CustomerName,
                invoice.CustomerTaxNumber,
                gross,
                invoice.CurrencyCode),
            cancellationToken);

        if (result.Succeeded && result.DocumentUuid is not null && result.Ettn is not null)
        {
            _salesInvoiceDirectoryService.ApplyElectronicDocumentResult(
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
        var invoice = _salesInvoiceDirectoryService.GetById(id);

        if (invoice is null || string.IsNullOrWhiteSpace(invoice.DocumentUuid))
        {
            TempData["InvoiceSaved"] = "Sorgulanacak e-belge bulunamadi.";
            return RedirectToAction(nameof(Details), new { id });
        }

        var result = await _electronicDocumentIntegration.GetStatusAsync(invoice.DocumentUuid, cancellationToken);
        _salesInvoiceDirectoryService.ApplyElectronicDocumentResult(id, result.DocumentUuid, result.Ettn ?? invoice.Ettn ?? "", result.Status);
        TempData["InvoiceSaved"] = result.Message;
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CancelElectronicDocument(Guid id, CancellationToken cancellationToken)
    {
        var invoice = _salesInvoiceDirectoryService.GetById(id);

        if (invoice is null || string.IsNullOrWhiteSpace(invoice.DocumentUuid))
        {
            TempData["InvoiceSaved"] = "Iptal edilecek e-belge bulunamadi.";
            return RedirectToAction(nameof(Details), new { id });
        }

        var result = await _electronicDocumentIntegration.CancelAsync(invoice.DocumentUuid, cancellationToken);

        if (result.Succeeded)
        {
            _salesInvoiceDirectoryService.ApplyElectronicDocumentResult(
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
        var deleted = _salesInvoiceDirectoryService.Delete(id);

        if (!deleted)
        {
            return NotFound();
        }

        TempData["InvoiceSaved"] = "Satis faturasi silindi.";
        return RedirectToAction(nameof(Index));
    }
}
