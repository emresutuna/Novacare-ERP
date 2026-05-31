namespace NovacareERP.Infrastructure.Persistence.Entities;

public sealed class InvoiceLineRecord
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid InvoiceId { get; set; }
    public InvoiceRecord? Invoice { get; set; }
    public string ProductOrServiceName { get; set; } = "";
    public decimal Quantity { get; set; } = 1;
    public decimal UnitPrice { get; set; }
    public decimal TaxRate { get; set; } = 20;
    public decimal TotalAmount { get; set; }
    public int SortOrder { get; set; }
}
