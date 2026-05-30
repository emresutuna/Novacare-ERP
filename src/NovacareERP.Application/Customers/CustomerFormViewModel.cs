namespace NovacareERP.Application.Customers;

public sealed class CustomerFormViewModel
{
    public string Title { get; init; } = "Yeni Musteri";
    public string DisplayName { get; init; } = "";
    public NovacareERP.Domain.Enums.CustomerLegalType LegalType { get; init; } = NovacareERP.Domain.Enums.CustomerLegalType.LimitedCompany;
    public string TaxNumber { get; init; } = "";
    public string TaxOffice { get; init; } = "";
    public string ContactPerson { get; init; } = "";
    public string ContactTitle { get; init; } = "";
    public string Email { get; init; } = "";
    public string Phone { get; init; } = "";
    public string City { get; init; } = "";
    public string Address { get; init; } = "";
    public string CurrencyCode { get; init; } = "TRY";
    public decimal CreditLimit { get; init; } = 50000;
    public int PaymentTermDays { get; init; } = 30;
}
