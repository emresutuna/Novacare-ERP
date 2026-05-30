using NovacareERP.Application.Invoices;
using NovacareERP.Application.PurchaseInvoices;
using NovacareERP.Application.Suppliers;
using NovacareERP.Domain.Enums;

namespace NovacareERP.Infrastructure.Persistence;

public sealed class InMemoryPurchaseInvoiceDirectoryService : IPurchaseInvoiceDirectoryService
{
    private readonly ISupplierDirectoryService _supplierDirectoryService;
    private readonly List<PurchaseInvoiceStorageItem> _invoices =
    [
        new(
            Id: Guid.Parse("b1c2d3e4-f5a6-7890-abcd-ef1234567001"),
            SupplierId: null,
            SupplierName: "Atlas Bilisim Ltd.",
            SupplierTaxNumber: "6123456789",
            InvoiceNumber: "AF-2026-0001",
            IssueDate: new DateOnly(2026, 5, 27),
            DueDate: new DateOnly(2026, 6, 27),
            CurrencyCode: "TRY",
            ExchangeRate: 1,
            Description: "Yazilim lisans alimi",
            Status: InvoiceStatus.Issued,
            DocumentType: ElectronicDocumentType.EFatura,
            DocumentStatus: ElectronicDocumentStatus.Accepted,
            DocumentUuid: "22222222-3333-4444-5555-666666666601",
            Ettn: "GELENEFATURA1234",
            Items:
            [
                new() { ProductOrServiceName = "ERP Lisans", Quantity = 1, UnitPrice = 85000, TaxRate = 20, TotalAmount = 85000 }
            ]),
        new(
            Id: Guid.Parse("b1c2d3e4-f5a6-7890-abcd-ef1234567002"),
            SupplierId: null,
            SupplierName: "Kocak Ofis Malzemeleri",
            SupplierTaxNumber: "9988776655",
            InvoiceNumber: "AF-2026-0002",
            IssueDate: new DateOnly(2026, 5, 29),
            DueDate: new DateOnly(2026, 6, 29),
            CurrencyCode: "TRY",
            ExchangeRate: 1,
            Description: "Ofis sarf malzemeleri",
            Status: InvoiceStatus.Draft,
            DocumentType: ElectronicDocumentType.None,
            DocumentStatus: ElectronicDocumentStatus.NotSent,
            DocumentUuid: null,
            Ettn: null,
            Items:
            [
                new() { ProductOrServiceName = "A4 Kagit", Quantity = 10, UnitPrice = 120, TaxRate = 20, TotalAmount = 1200 },
                new() { ProductOrServiceName = "Toner Kartus", Quantity = 2, UnitPrice = 1800, TaxRate = 20, TotalAmount = 3600 }
            ])
    ];

    private int _invoiceSequence = 2;

    public InMemoryPurchaseInvoiceDirectoryService(ISupplierDirectoryService supplierDirectoryService)
    {
        _supplierDirectoryService = supplierDirectoryService;
    }

    public PurchaseInvoiceListViewModel GetList(string? searchTerm, InvoiceStatus? status, ElectronicDocumentType? documentType)
    {
        var query = _invoices.AsEnumerable();

        if (status.HasValue)
        {
            query = query.Where(invoice => invoice.Status == status.Value);
        }

        if (documentType.HasValue)
        {
            query = query.Where(invoice => invoice.DocumentType == documentType.Value);
        }

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(invoice =>
                invoice.SupplierName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                invoice.InvoiceNumber.Contains(searchTerm, StringComparison.OrdinalIgnoreCase));
        }

        return new PurchaseInvoiceListViewModel
        {
            Invoices = query
                .OrderByDescending(invoice => invoice.IssueDate)
                .Select(ToListItem)
                .ToList(),
            SearchTerm = searchTerm,
            Status = status,
            DocumentType = documentType
        };
    }

    public PurchaseInvoiceFormViewModel? GetById(Guid id)
    {
        var invoice = _invoices.FirstOrDefault(item => item.Id == id);
        return invoice is null ? null : ToForm(invoice);
    }

    public PurchaseInvoiceFormViewModel GetDraft(Guid? supplierId = null, ElectronicDocumentType? documentType = null)
    {
        var supplier = supplierId.HasValue ? _supplierDirectoryService.GetById(supplierId.Value) : null;
        _invoiceSequence++;

        return new PurchaseInvoiceFormViewModel
        {
            SupplierId = supplier?.Id,
            SupplierName = supplier?.DisplayName ?? "",
            SupplierTaxNumber = supplier?.TaxNumber ?? "",
            InvoiceNumber = $"AF-2026-{_invoiceSequence:0000}",
            IssueDate = DateOnly.FromDateTime(DateTime.Today),
            DueDate = DateOnly.FromDateTime(DateTime.Today.AddDays(30)),
            DocumentType = documentType ?? ElectronicDocumentType.EFatura,
            Items =
            [
                new() { ProductOrServiceName = "Urun / Hizmet", Quantity = 1, UnitPrice = 0, TaxRate = 20, TotalAmount = 0 }
            ]
        };
    }

    public Guid Add(PurchaseInvoiceFormViewModel form)
    {
        var id = Guid.NewGuid();
        var items = form.Items.Count == 0
            ? [new InvoiceLineItemViewModel { ProductOrServiceName = "Urun / Hizmet", Quantity = 1, UnitPrice = 0, TaxRate = 20, TotalAmount = 0 }]
            : form.Items.ToList();

        _invoices.Add(new PurchaseInvoiceStorageItem(
            id,
            form.SupplierId,
            Clean(form.SupplierName),
            Clean(form.SupplierTaxNumber),
            Clean(form.InvoiceNumber),
            form.IssueDate,
            form.DueDate,
            Clean(form.CurrencyCode),
            form.ExchangeRate,
            Clean(form.Description),
            form.Status,
            form.DocumentType,
            form.DocumentStatus,
            form.DocumentUuid,
            form.Ettn,
            items));

        return id;
    }

    public bool Update(Guid id, PurchaseInvoiceFormViewModel form)
    {
        var index = _invoices.FindIndex(invoice => invoice.Id == id);

        if (index < 0)
        {
            return false;
        }

        var current = _invoices[index];
        _invoices[index] = current with
        {
            SupplierId = form.SupplierId,
            SupplierName = Clean(form.SupplierName),
            SupplierTaxNumber = Clean(form.SupplierTaxNumber),
            InvoiceNumber = Clean(form.InvoiceNumber),
            IssueDate = form.IssueDate,
            DueDate = form.DueDate,
            CurrencyCode = Clean(form.CurrencyCode),
            ExchangeRate = form.ExchangeRate,
            Description = Clean(form.Description),
            Status = form.Status,
            DocumentType = form.DocumentType,
            DocumentStatus = form.DocumentStatus,
            DocumentUuid = form.DocumentUuid,
            Ettn = form.Ettn,
            Items = form.Items.Count == 0 ? current.Items : form.Items.ToList()
        };

        return true;
    }

    public bool Delete(Guid id)
    {
        var invoice = _invoices.FirstOrDefault(item => item.Id == id);

        if (invoice is null)
        {
            return false;
        }

        _invoices.Remove(invoice);
        return true;
    }

    public bool ApplyElectronicDocumentResult(Guid id, string documentUuid, string ettn, ElectronicDocumentStatus status)
    {
        var index = _invoices.FindIndex(invoice => invoice.Id == id);

        if (index < 0)
        {
            return false;
        }

        var current = _invoices[index];
        _invoices[index] = current with
        {
            DocumentUuid = documentUuid,
            Ettn = ettn,
            DocumentStatus = status,
            Status = status == ElectronicDocumentStatus.Accepted ? InvoiceStatus.Issued : current.Status
        };

        return true;
    }

    private static PurchaseInvoiceListItemViewModel ToListItem(PurchaseInvoiceStorageItem invoice)
    {
        var gross = CalculateGross(invoice.Items);

        return new PurchaseInvoiceListItemViewModel(
            invoice.Id,
            invoice.IssueDate,
            invoice.SupplierName,
            invoice.InvoiceNumber,
            gross,
            invoice.CurrencyCode,
            invoice.Status,
            InvoiceDisplayText.GetStatusText(invoice.Status),
            invoice.DocumentType,
            InvoiceDisplayText.GetDocumentTypeText(invoice.DocumentType),
            invoice.DocumentStatus,
            InvoiceDisplayText.GetDocumentStatusText(invoice.DocumentStatus));
    }

    private static PurchaseInvoiceFormViewModel ToForm(PurchaseInvoiceStorageItem invoice) =>
        new()
        {
            Id = invoice.Id,
            Title = "Alis Faturasi Duzenle",
            SupplierId = invoice.SupplierId,
            SupplierName = invoice.SupplierName,
            SupplierTaxNumber = invoice.SupplierTaxNumber,
            InvoiceNumber = invoice.InvoiceNumber,
            IssueDate = invoice.IssueDate,
            DueDate = invoice.DueDate,
            CurrencyCode = invoice.CurrencyCode,
            ExchangeRate = invoice.ExchangeRate,
            Description = invoice.Description,
            Status = invoice.Status,
            DocumentType = invoice.DocumentType,
            DocumentStatus = invoice.DocumentStatus,
            DocumentUuid = invoice.DocumentUuid,
            Ettn = invoice.Ettn,
            Items = invoice.Items
        };

    private static decimal CalculateGross(IReadOnlyList<InvoiceLineItemViewModel> items) =>
        items.Sum(item => item.TotalAmount + item.TotalAmount * item.TaxRate / 100m);

    private static string Clean(string value) => value.Trim();

    private sealed record PurchaseInvoiceStorageItem(
        Guid Id,
        Guid? SupplierId,
        string SupplierName,
        string SupplierTaxNumber,
        string InvoiceNumber,
        DateOnly IssueDate,
        DateOnly? DueDate,
        string CurrencyCode,
        decimal ExchangeRate,
        string Description,
        InvoiceStatus Status,
        ElectronicDocumentType DocumentType,
        ElectronicDocumentStatus DocumentStatus,
        string? DocumentUuid,
        string? Ettn,
        IReadOnlyList<InvoiceLineItemViewModel> Items);
}
