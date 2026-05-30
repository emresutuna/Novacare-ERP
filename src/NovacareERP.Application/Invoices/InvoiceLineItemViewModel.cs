namespace NovacareERP.Application.Invoices;

public sealed class InvoiceLineItemViewModel
{
    public string ProductOrServiceName { get; init; } = "";
    public decimal Quantity { get; init; }
    public decimal UnitPrice { get; init; }
    public decimal TaxRate { get; init; }
    public decimal TotalAmount { get; init; }
}
