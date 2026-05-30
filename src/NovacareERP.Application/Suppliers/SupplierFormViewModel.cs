using NovacareERP.Domain.Enums;

namespace NovacareERP.Application.Suppliers;

public sealed class SupplierFormViewModel
{
    public Guid Id { get; init; }
    public string Title { get; init; } = "Yeni Tedarikci";
    public string DisplayName { get; init; } = "";
    public CustomerLegalType LegalType { get; init; } = CustomerLegalType.LimitedCompany;
    public string TaxNumber { get; init; } = "";
    public string TaxOffice { get; init; } = "";
    public string ContactPerson { get; init; } = "";
    public string ContactTitle { get; init; } = "";
    public string Email { get; init; } = "";
    public string Phone { get; init; } = "";
    public string City { get; init; } = "";
    public string Address { get; init; } = "";
    public string CurrencyCode { get; init; } = "TRY";
    public decimal PurchaseLimit { get; init; } = 100000;
    public int PaymentTermDays { get; init; } = 30;
}
