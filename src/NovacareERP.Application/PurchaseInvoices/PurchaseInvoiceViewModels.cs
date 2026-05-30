using NovacareERP.Application.Invoices;
using NovacareERP.Domain.Enums;

namespace NovacareERP.Application.PurchaseInvoices;

public sealed class PurchaseInvoiceListViewModel
{
    public IReadOnlyList<PurchaseInvoiceListItemViewModel> Invoices { get; init; } = [];
    public string? SearchTerm { get; init; }
    public InvoiceStatus? Status { get; init; }
    public ElectronicDocumentType? DocumentType { get; init; }
    public IReadOnlyList<DocumentTypeOption> DocumentTypeOptions { get; init; } = InvoiceDisplayText.GetPurchaseDocumentTypeOptions();
}

public sealed record PurchaseInvoiceListItemViewModel(
    Guid Id,
    DateOnly IssueDate,
    string SupplierName,
    string InvoiceNumber,
    decimal GrossAmount,
    string CurrencyCode,
    InvoiceStatus Status,
    string StatusText,
    ElectronicDocumentType DocumentType,
    string DocumentTypeText,
    ElectronicDocumentStatus DocumentStatus,
    string DocumentStatusText);

public sealed class PurchaseInvoiceFormViewModel
{
    public Guid Id { get; init; }
    public string Title { get; init; } = "Yeni Alis Faturasi";
    public Guid? SupplierId { get; init; }
    public string SupplierName { get; init; } = "";
    public string SupplierTaxNumber { get; init; } = "";
    public string InvoiceNumber { get; init; } = "";
    public DateOnly IssueDate { get; init; } = DateOnly.FromDateTime(DateTime.Today);
    public DateOnly? DueDate { get; init; }
    public string CurrencyCode { get; init; } = "TRY";
    public decimal ExchangeRate { get; init; } = 1;
    public string Description { get; init; } = "";
    public InvoiceStatus Status { get; init; } = InvoiceStatus.Draft;
    public ElectronicDocumentType DocumentType { get; init; } = ElectronicDocumentType.EFatura;
    public ElectronicDocumentStatus DocumentStatus { get; init; } = ElectronicDocumentStatus.NotSent;
    public string? DocumentUuid { get; init; }
    public string? Ettn { get; init; }
    public string IntegrationNote { get; init; } = "Gelen e-faturalar Finora entegrasyonu ile otomatik eslestirilecektir.";
    public IReadOnlyList<InvoiceLineItemViewModel> Items { get; init; } = [];
    public IReadOnlyList<DocumentTypeOption> DocumentTypeOptions { get; init; } = InvoiceDisplayText.GetPurchaseDocumentTypeOptions();
}
