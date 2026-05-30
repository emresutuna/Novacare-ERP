using NovacareERP.Domain.Common;

namespace NovacareERP.Domain.Entities;

public sealed class Company : BaseEntity
{
    public Company(string name, string taxNumber, string taxOffice, string defaultCurrencyCode = "TRY")
    {
        Name = name;
        TaxNumber = taxNumber;
        TaxOffice = taxOffice;
        DefaultCurrencyCode = defaultCurrencyCode;
    }

    public string Name { get; private set; }
    public string TaxNumber { get; private set; }
    public string TaxOffice { get; private set; }
    public string DefaultCurrencyCode { get; private set; }
    public string InvoicePrefix { get; private set; } = "NVC";
    public int CurrentInvoiceNumber { get; private set; } = 1;
}
