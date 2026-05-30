namespace NovacareERP.Domain.Entities;

public sealed class InvoiceItem
{
    public InvoiceItem(string description, decimal quantity, decimal unitPrice, decimal taxRate)
    {
        Description = description;
        Quantity = quantity;
        UnitPrice = unitPrice;
        TaxRate = taxRate;
    }

    public string Description { get; }
    public decimal Quantity { get; }
    public decimal UnitPrice { get; }
    public decimal TaxRate { get; }
    public decimal NetAmount => decimal.Round(Quantity * UnitPrice, 2);
    public decimal TaxAmount => decimal.Round(NetAmount * TaxRate / 100, 2);
    public decimal GrossAmount => NetAmount + TaxAmount;
}
