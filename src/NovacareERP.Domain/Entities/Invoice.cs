using NovacareERP.Domain.Common;
using NovacareERP.Domain.Enums;

namespace NovacareERP.Domain.Entities;

public sealed class Invoice : BaseEntity, ICompanyScoped
{
    private readonly List<InvoiceItem> _items = [];

    public Invoice(Guid companyId, Guid currentAccountId, string invoiceNumber, DateOnly issueDate)
    {
        CompanyId = companyId;
        CurrentAccountId = currentAccountId;
        InvoiceNumber = invoiceNumber;
        IssueDate = issueDate;
    }

    public Guid CompanyId { get; private set; }
    public Guid CurrentAccountId { get; private set; }
    public string InvoiceNumber { get; private set; }
    public DateOnly IssueDate { get; private set; }
    public DateOnly? DueDate { get; private set; }
    public InvoiceStatus Status { get; private set; } = InvoiceStatus.Draft;
    public decimal NetAmount => _items.Sum(item => item.NetAmount);
    public decimal TaxAmount => _items.Sum(item => item.TaxAmount);
    public decimal GrossAmount => _items.Sum(item => item.GrossAmount);
    public IReadOnlyCollection<InvoiceItem> Items => _items;

    public void AddItem(string description, decimal quantity, decimal unitPrice, decimal taxRate)
    {
        _items.Add(new InvoiceItem(description, quantity, unitPrice, taxRate));
        MarkUpdated();
    }

    public void Issue()
    {
        if (_items.Count == 0)
        {
            throw new InvalidOperationException("Invoice must have at least one item.");
        }

        Status = InvoiceStatus.Issued;
        MarkUpdated();
    }
}
