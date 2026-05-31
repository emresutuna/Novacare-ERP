using NovacareERP.Domain.Enums;

namespace NovacareERP.Infrastructure.Persistence.Entities;

public sealed class InvoiceRecord
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public InvoiceType InvoiceType { get; set; }
    public Guid? CustomerId { get; set; }
    public Guid? SupplierId { get; set; }
    public string PartyName { get; set; } = "";
    public string PartyTaxNumber { get; set; } = "";
    public string InvoiceNumber { get; set; } = "";
    public DateOnly IssueDate { get; set; } = DateOnly.FromDateTime(DateTime.Today);
    public DateOnly? DueDate { get; set; }
    public string CurrencyCode { get; set; } = "TRY";
    public decimal ExchangeRate { get; set; } = 1;
    public string Description { get; set; } = "";
    public InvoiceStatus Status { get; set; } = InvoiceStatus.Draft;
    public ElectronicDocumentType DocumentType { get; set; } = ElectronicDocumentType.EFatura;
    public ElectronicDocumentStatus DocumentStatus { get; set; } = ElectronicDocumentStatus.NotSent;
    public string? DocumentUuid { get; set; }
    public string? Ettn { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public List<InvoiceLineRecord> Items { get; set; } = [];
}
