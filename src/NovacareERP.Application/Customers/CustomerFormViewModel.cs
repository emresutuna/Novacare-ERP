namespace NovacareERP.Application.Customers;

public sealed class CustomerFormViewModel
{
    public string Title { get; init; } = "Yeni Cari Hesap";
    public string DisplayName { get; init; } = "Eyup Biradli";
    public string TaxNumber { get; init; } = "";
    public string TaxOffice { get; init; } = "";
    public string Email { get; init; } = "";
    public string Phone { get; init; } = "";
    public string Address { get; init; } = "";
    public decimal CreditLimit { get; init; } = 50000;
    public int PaymentTermDays { get; init; } = 30;
}
