using NovacareERP.Application.Customers;
using NovacareERP.Application.Invoices;
using NovacareERP.Application.SalesInvoices;
using NovacareERP.Domain.Enums;

namespace NovacareERP.Infrastructure.Persistence;

public sealed class InMemorySalesInvoiceDirectoryService : ISalesInvoiceDirectoryService
{
    private readonly ICustomerDirectoryService _customerDirectoryService;
    private readonly List<SalesInvoiceStorageItem> _invoices =
    [
        new(
            Id: Guid.Parse("a1b2c3d4-e5f6-7890-abcd-ef1234567001"),
            CustomerId: Guid.Parse("5b6c6cdd-3ac8-4ad3-8e72-bac1f31dd001"),
            CustomerName: "Nova Medikal A.S.",
            CustomerTaxNumber: "4890123456",
            InvoiceNumber: "SF-2026-0001",
            IssueDate: new DateOnly(2026, 5, 28),
            DueDate: new DateOnly(2026, 6, 28),
            CurrencyCode: "TRY",
            ExchangeRate: 1,
            Description: "Medikal cihaz satisi",
            Status: InvoiceStatus.Issued,
            DocumentType: ElectronicDocumentType.EFatura,
            DocumentStatus: ElectronicDocumentStatus.Accepted,
            DocumentUuid: "11111111-2222-3333-4444-555555555501",
            Ettn: "A1B2C3D4E5F67890",
            Items:
            [
                new() { ProductOrServiceName = "Cerrahi Set", Quantity = 2, UnitPrice = 45000, TaxRate = 20, TotalAmount = 90000 },
                new() { ProductOrServiceName = "Sterilizasyon Hizmeti", Quantity = 1, UnitPrice = 12500, TaxRate = 20, TotalAmount = 12500 }
            ]),
        new(
            Id: Guid.Parse("a1b2c3d4-e5f6-7890-abcd-ef1234567002"),
            CustomerId: Guid.Parse("5b6c6cdd-3ac8-4ad3-8e72-bac1f31dd002"),
            CustomerName: "Ayse Demir",
            CustomerTaxNumber: "11111111111",
            InvoiceNumber: "SF-2026-0002",
            IssueDate: new DateOnly(2026, 5, 29),
            DueDate: new DateOnly(2026, 6, 29),
            CurrencyCode: "TRY",
            ExchangeRate: 1,
            Description: "Perakende urun satisi",
            Status: InvoiceStatus.Draft,
            DocumentType: ElectronicDocumentType.EArsiv,
            DocumentStatus: ElectronicDocumentStatus.NotSent,
            DocumentUuid: null,
            Ettn: null,
            Items:
            [
                new() { ProductOrServiceName = "Microsoft Mouse", Quantity = 1, UnitPrice = 2500, TaxRate = 20, TotalAmount = 2500 }
            ]),
        new(
            Id: Guid.Parse("a1b2c3d4-e5f6-7890-abcd-ef1234567003"),
            CustomerId: Guid.Parse("5b6c6cdd-3ac8-4ad3-8e72-bac1f31dd003"),
            CustomerName: "Sutuna Danismanlik",
            CustomerTaxNumber: "12345678901",
            InvoiceNumber: "SF-2026-0003",
            IssueDate: new DateOnly(2026, 5, 30),
            DueDate: new DateOnly(2026, 5, 30),
            CurrencyCode: "EUR",
            ExchangeRate: 35.5m,
            Description: "Sevkiyat irsaliyesi",
            Status: InvoiceStatus.Issued,
            DocumentType: ElectronicDocumentType.EIrsaliye,
            DocumentStatus: ElectronicDocumentStatus.Pending,
            DocumentUuid: "11111111-2222-3333-4444-555555555503",
            Ettn: "EIRSALIYE1234567",
            Items:
            [
                new() { ProductOrServiceName = "Danismanlik Paketi", Quantity = 1, UnitPrice = 5000, TaxRate = 20, TotalAmount = 5000 }
            ])
    ];

    private int _invoiceSequence = 3;

    public InMemorySalesInvoiceDirectoryService(ICustomerDirectoryService customerDirectoryService)
    {
        _customerDirectoryService = customerDirectoryService;
    }

    public SalesInvoiceListViewModel GetList(string? searchTerm, InvoiceStatus? status, ElectronicDocumentType? documentType)
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
                invoice.CustomerName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                invoice.InvoiceNumber.Contains(searchTerm, StringComparison.OrdinalIgnoreCase));
        }

        return new SalesInvoiceListViewModel
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

    public SalesInvoiceFormViewModel? GetById(Guid id)
    {
        var invoice = _invoices.FirstOrDefault(item => item.Id == id);
        return invoice is null ? null : ToForm(invoice);
    }

    public SalesInvoiceFormViewModel GetDraft(Guid? customerId = null, ElectronicDocumentType? documentType = null)
    {
        var customer = customerId.HasValue ? _customerDirectoryService.GetById(customerId.Value) : null;
        _invoiceSequence++;

        return new SalesInvoiceFormViewModel
        {
            CustomerId = customer?.Id,
            CustomerName = customer?.DisplayName ?? "",
            CustomerTaxNumber = customer?.TaxNumber ?? "",
            InvoiceNumber = $"SF-2026-{_invoiceSequence:0000}",
            IssueDate = DateOnly.FromDateTime(DateTime.Today),
            DueDate = DateOnly.FromDateTime(DateTime.Today.AddDays(30)),
            DocumentType = documentType ?? ElectronicDocumentType.EFatura,
            Items =
            [
                new() { ProductOrServiceName = "Urun / Hizmet", Quantity = 1, UnitPrice = 0, TaxRate = 20, TotalAmount = 0 }
            ]
        };
    }

    public Guid Add(SalesInvoiceFormViewModel form)
    {
        var id = Guid.NewGuid();
        var items = form.Items.Count == 0
            ? [new InvoiceLineItemViewModel { ProductOrServiceName = "Urun / Hizmet", Quantity = 1, UnitPrice = 0, TaxRate = 20, TotalAmount = 0 }]
            : form.Items.ToList();

        _invoices.Add(new SalesInvoiceStorageItem(
            id,
            form.CustomerId,
            Clean(form.CustomerName),
            Clean(form.CustomerTaxNumber),
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

    public bool Update(Guid id, SalesInvoiceFormViewModel form)
    {
        var index = _invoices.FindIndex(invoice => invoice.Id == id);

        if (index < 0)
        {
            return false;
        }

        var current = _invoices[index];
        _invoices[index] = current with
        {
            CustomerId = form.CustomerId,
            CustomerName = Clean(form.CustomerName),
            CustomerTaxNumber = Clean(form.CustomerTaxNumber),
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

    private static SalesInvoiceListItemViewModel ToListItem(SalesInvoiceStorageItem invoice)
    {
        var gross = CalculateGross(invoice.Items);

        return new SalesInvoiceListItemViewModel(
            invoice.Id,
            invoice.IssueDate,
            invoice.CustomerName,
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

    private static SalesInvoiceFormViewModel ToForm(SalesInvoiceStorageItem invoice) =>
        new()
        {
            Id = invoice.Id,
            Title = "Satis Faturasi Duzenle",
            CustomerId = invoice.CustomerId,
            CustomerName = invoice.CustomerName,
            CustomerTaxNumber = invoice.CustomerTaxNumber,
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

    private sealed record SalesInvoiceStorageItem(
        Guid Id,
        Guid? CustomerId,
        string CustomerName,
        string CustomerTaxNumber,
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
