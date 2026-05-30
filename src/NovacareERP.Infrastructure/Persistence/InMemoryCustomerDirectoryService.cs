using NovacareERP.Application.Customers;
using NovacareERP.Domain.Enums;

namespace NovacareERP.Infrastructure.Persistence;

public sealed class InMemoryCustomerDirectoryService : ICustomerDirectoryService
{
    private readonly List<CustomerListItemViewModel> _customers =
    [
        new(
            Id: Guid.Parse("5b6c6cdd-3ac8-4ad3-8e72-bac1f31dd001"),
            Code: "MUS-0001",
            DisplayName: "Nova Medikal A.S.",
            LegalType: CustomerLegalType.JointStockCompany,
            LegalTypeText: GetLegalTypeText(CustomerLegalType.JointStockCompany),
            TaxNumber: "4890123456",
            TaxOffice: "Kadikoy",
            ContactPerson: "Derya Yilmaz",
            ContactTitle: "Finans Sorumlusu",
            Email: "finans@novamedikal.com",
            Phone: "+90 216 000 10 20",
            City: "Istanbul",
            CurrencyCode: "TRY",
            Balance: 22500,
            CreditLimit: 150000,
            IsActive: true),
        new(
            Id: Guid.Parse("5b6c6cdd-3ac8-4ad3-8e72-bac1f31dd002"),
            Code: "MUS-0002",
            DisplayName: "Ayse Demir",
            LegalType: CustomerLegalType.EndUser,
            LegalTypeText: GetLegalTypeText(CustomerLegalType.EndUser),
            TaxNumber: "11111111111",
            TaxOffice: "",
            ContactPerson: "Ayse Demir",
            ContactTitle: "Son Kullanici",
            Email: "ayse.demir@example.com",
            Phone: "+90 555 111 22 33",
            City: "Ankara",
            CurrencyCode: "TRY",
            Balance: 8400,
            CreditLimit: 25000,
            IsActive: true),
        new(
            Id: Guid.Parse("5b6c6cdd-3ac8-4ad3-8e72-bac1f31dd003"),
            Code: "MUS-0003",
            DisplayName: "Sutuna Danismanlik",
            LegalType: CustomerLegalType.SoleProprietorship,
            LegalTypeText: GetLegalTypeText(CustomerLegalType.SoleProprietorship),
            TaxNumber: "12345678901",
            TaxOffice: "Besiktas",
            ContactPerson: "Emre Sutuna",
            ContactTitle: "Firma Yetkilisi",
            Email: "emre@sutunadanismanlik.com",
            Phone: "+90 532 222 44 55",
            City: "Istanbul",
            CurrencyCode: "EUR",
            Balance: 0,
            CreditLimit: 50000,
            IsActive: true)
    ];

    public CustomerListViewModel GetList(CustomerLegalType? legalType, string? searchTerm)
    {
        var query = _customers.AsEnumerable();

        if (legalType.HasValue)
        {
            query = query.Where(customer => customer.LegalType == legalType.Value);
        }

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(customer =>
                customer.DisplayName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                customer.Code.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                customer.ContactPerson.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                customer.TaxNumber.Contains(searchTerm, StringComparison.OrdinalIgnoreCase));
        }

        return new CustomerListViewModel
        {
            Customers = query.OrderBy(customer => customer.DisplayName).ToList(),
            Form = new CustomerFormViewModel(),
            LegalTypeOptions = GetLegalTypeOptions(),
            SelectedLegalType = legalType,
            SearchTerm = searchTerm
        };
    }

    public CustomerListItemViewModel? GetById(Guid id)
    {
        return _customers.FirstOrDefault(customer => customer.Id == id);
    }

    public void Add(CustomerFormViewModel form)
    {
        var legalTypeText = GetLegalTypeText(form.LegalType);
        var displayName = string.IsNullOrWhiteSpace(form.DisplayName)
            ? $"Yeni {legalTypeText}"
            : Clean(form.DisplayName);

        _customers.Add(new CustomerListItemViewModel(
            Id: Guid.NewGuid(),
            Code: $"MUS-{_customers.Count + 1:0000}",
            DisplayName: displayName,
            LegalType: form.LegalType,
            LegalTypeText: legalTypeText,
            TaxNumber: Clean(form.TaxNumber),
            TaxOffice: Clean(form.TaxOffice),
            ContactPerson: Clean(form.ContactPerson),
            ContactTitle: Clean(form.ContactTitle),
            Email: Clean(form.Email),
            Phone: Clean(form.Phone),
            City: Clean(form.City),
            CurrencyCode: NormalizeCurrencyCode(form.CurrencyCode),
            Balance: 0,
            CreditLimit: form.CreditLimit,
            IsActive: true));
    }

    public bool Update(Guid id, CustomerFormViewModel form)
    {
        var index = _customers.FindIndex(customer => customer.Id == id);

        if (index < 0)
        {
            return false;
        }

        var existing = _customers[index];
        var legalTypeText = GetLegalTypeText(form.LegalType);
        var displayName = string.IsNullOrWhiteSpace(form.DisplayName)
            ? existing.DisplayName
            : Clean(form.DisplayName);

        _customers[index] = existing with
        {
            DisplayName = displayName,
            LegalType = form.LegalType,
            LegalTypeText = legalTypeText,
            TaxNumber = Clean(form.TaxNumber),
            TaxOffice = Clean(form.TaxOffice),
            ContactPerson = Clean(form.ContactPerson),
            ContactTitle = Clean(form.ContactTitle),
            Email = Clean(form.Email),
            Phone = Clean(form.Phone),
            City = Clean(form.City),
            CurrencyCode = NormalizeCurrencyCode(form.CurrencyCode),
            CreditLimit = form.CreditLimit
        };

        return true;
    }

    public bool Delete(Guid id)
    {
        var customer = GetById(id);

        if (customer is null)
        {
            return false;
        }

        return _customers.Remove(customer);
    }

    private static string Clean(string? value)
    {
        return value?.Trim() ?? "";
    }

    private static string NormalizeCurrencyCode(string? value)
    {
        var currencyCode = Clean(value).ToUpperInvariant();
        return currencyCode is "TRY" or "USD" or "EUR" ? currencyCode : "TRY";
    }

    private static IReadOnlyList<CustomerLegalTypeOption> GetLegalTypeOptions()
    {
        return
        [
            new(CustomerLegalType.EndUser, GetLegalTypeText(CustomerLegalType.EndUser)),
            new(CustomerLegalType.SoleProprietorship, GetLegalTypeText(CustomerLegalType.SoleProprietorship)),
            new(CustomerLegalType.LimitedCompany, GetLegalTypeText(CustomerLegalType.LimitedCompany)),
            new(CustomerLegalType.JointStockCompany, GetLegalTypeText(CustomerLegalType.JointStockCompany)),
            new(CustomerLegalType.Cooperative, GetLegalTypeText(CustomerLegalType.Cooperative)),
            new(CustomerLegalType.Association, GetLegalTypeText(CustomerLegalType.Association)),
            new(CustomerLegalType.Foundation, GetLegalTypeText(CustomerLegalType.Foundation)),
            new(CustomerLegalType.Other, GetLegalTypeText(CustomerLegalType.Other))
        ];
    }

    private static string GetLegalTypeText(CustomerLegalType legalType)
    {
        return legalType switch
        {
            CustomerLegalType.EndUser => "Son Kullanici",
            CustomerLegalType.SoleProprietorship => "Sahis Sirketi",
            CustomerLegalType.LimitedCompany => "Limited Sirket",
            CustomerLegalType.JointStockCompany => "Anonim Sirket",
            CustomerLegalType.Cooperative => "Kooperatif",
            CustomerLegalType.Association => "Dernek",
            CustomerLegalType.Foundation => "Vakif",
            _ => "Diger"
        };
    }
}
