using NovacareERP.Domain.Enums;

namespace NovacareERP.Application.Invoices;

public sealed record DocumentTypeOption(ElectronicDocumentType Value, string Text);

public static class InvoiceDisplayText
{
    public static string GetStatusText(InvoiceStatus status) => status switch
    {
        InvoiceStatus.Draft => "Taslak",
        InvoiceStatus.Issued => "Kesildi",
        InvoiceStatus.PartiallyPaid => "Kismi Odendi",
        InvoiceStatus.Paid => "Odendi",
        InvoiceStatus.Cancelled => "Iptal",
        _ => status.ToString()
    };

    public static string GetDocumentTypeText(ElectronicDocumentType documentType) => documentType switch
    {
        ElectronicDocumentType.EFatura => "E-Fatura",
        ElectronicDocumentType.EArsiv => "E-Arsiv",
        ElectronicDocumentType.EIrsaliye => "E-Irsaliye",
        ElectronicDocumentType.None => "Kagit Fatura",
        _ => documentType.ToString()
    };

    public static string GetDocumentStatusText(ElectronicDocumentStatus status) => status switch
    {
        ElectronicDocumentStatus.NotSent => "Gonderilmedi",
        ElectronicDocumentStatus.Pending => "GIB Bekliyor",
        ElectronicDocumentStatus.Accepted => "Onaylandi",
        ElectronicDocumentStatus.Rejected => "Reddedildi",
        ElectronicDocumentStatus.Cancelled => "Iptal Edildi",
        _ => status.ToString()
    };

    public static IReadOnlyList<DocumentTypeOption> GetSalesDocumentTypeOptions() =>
    [
        new(ElectronicDocumentType.EFatura, "E-Fatura"),
        new(ElectronicDocumentType.EArsiv, "E-Arsiv"),
        new(ElectronicDocumentType.EIrsaliye, "E-Irsaliye")
    ];

    public static IReadOnlyList<DocumentTypeOption> GetPurchaseDocumentTypeOptions() =>
    [
        new(ElectronicDocumentType.EFatura, "E-Fatura (Gelen)"),
        new(ElectronicDocumentType.None, "Kagit / Manuel Fatura")
    ];
}
