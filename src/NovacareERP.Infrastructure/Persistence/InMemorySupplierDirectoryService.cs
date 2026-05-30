using NovacareERP.Application.Customers;
using NovacareERP.Application.Suppliers;
using NovacareERP.Domain.Enums;

namespace NovacareERP.Infrastructure.Persistence;

public sealed class InMemorySupplierDirectoryService : ISupplierDirectoryService
{
    private readonly List<SupplierListItemViewModel> _suppliers = [];

    public SupplierListViewModel GetList(CustomerLegalType? legalType, string? searchTerm)
    {
        var query = _suppliers.AsEnumerable();

        if (legalType.HasValue)
        {
            query = query.Where(supplier => supplier.LegalType == legalType.Value);
        }

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(supplier =>
                supplier.DisplayName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                supplier.Code.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                supplier.ContactPerson.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                supplier.TaxNumber.Contains(searchTerm, StringComparison.OrdinalIgnoreCase));
        }

        return new SupplierListViewModel
        {
            Suppliers = query.OrderBy(supplier => supplier.DisplayName).ToList(),
            Form = new SupplierFormViewModel(),
            LegalTypeOptions = GetLegalTypeOptions(),
            SelectedLegalType = legalType,
            SearchTerm = searchTerm
        };
    }

    public SupplierListItemViewModel? GetById(Guid id)
    {
        return _suppliers.FirstOrDefault(supplier => supplier.Id == id);
    }

    public void Add(SupplierFormViewModel form)
    {
        var legalTypeText = GetLegalTypeText(form.LegalType);
        var displayName = string.IsNullOrWhiteSpace(form.DisplayName)
            ? $"Yeni {legalTypeText}"
            : Clean(form.DisplayName);

        _suppliers.Add(new SupplierListItemViewModel(
            Id: Guid.NewGuid(),
            Code: $"TED-{_suppliers.Count + 1:0000}",
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
            PurchaseLimit: form.PurchaseLimit,
            IsActive: true));
    }

    public bool Update(Guid id, SupplierFormViewModel form)
    {
        var index = _suppliers.FindIndex(supplier => supplier.Id == id);

        if (index < 0)
        {
            return false;
        }

        var existing = _suppliers[index];
        var legalTypeText = GetLegalTypeText(form.LegalType);
        var displayName = string.IsNullOrWhiteSpace(form.DisplayName)
            ? existing.DisplayName
            : Clean(form.DisplayName);

        _suppliers[index] = existing with
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
            PurchaseLimit = form.PurchaseLimit
        };

        return true;
    }

    public bool Delete(Guid id)
    {
        var supplier = GetById(id);

        if (supplier is null)
        {
            return false;
        }

        return _suppliers.Remove(supplier);
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
            new(CustomerLegalType.SoleProprietorship, GetLegalTypeText(CustomerLegalType.SoleProprietorship)),
            new(CustomerLegalType.LimitedCompany, GetLegalTypeText(CustomerLegalType.LimitedCompany)),
            new(CustomerLegalType.JointStockCompany, GetLegalTypeText(CustomerLegalType.JointStockCompany)),
            new(CustomerLegalType.Cooperative, GetLegalTypeText(CustomerLegalType.Cooperative)),
            new(CustomerLegalType.Other, GetLegalTypeText(CustomerLegalType.Other))
        ];
    }

    private static string GetLegalTypeText(CustomerLegalType legalType)
    {
        return legalType switch
        {
            CustomerLegalType.SoleProprietorship => "Sahis Sirketi",
            CustomerLegalType.LimitedCompany => "Limited Sirket",
            CustomerLegalType.JointStockCompany => "Anonim Sirket",
            CustomerLegalType.Cooperative => "Kooperatif",
            _ => "Diger"
        };
    }
}
