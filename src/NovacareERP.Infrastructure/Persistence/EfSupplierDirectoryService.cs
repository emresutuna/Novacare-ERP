using Microsoft.EntityFrameworkCore;
using NovacareERP.Application.Customers;
using NovacareERP.Application.Suppliers;
using NovacareERP.Domain.Enums;
using NovacareERP.Infrastructure.Persistence.Entities;

namespace NovacareERP.Infrastructure.Persistence;

public sealed class EfSupplierDirectoryService : ISupplierDirectoryService
{
    private readonly AppDbContext _dbContext;

    public EfSupplierDirectoryService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public SupplierListViewModel GetList(CustomerLegalType? legalType, string? searchTerm)
    {
        var query = _dbContext.Suppliers
            .AsNoTracking()
            .Where(supplier => !supplier.IsDeleted);

        if (legalType.HasValue)
        {
            query = query.Where(supplier => supplier.LegalType == legalType.Value);
        }

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.Trim();
            query = query.Where(supplier =>
                supplier.DisplayName.Contains(term) ||
                supplier.Code.Contains(term) ||
                supplier.ContactPerson.Contains(term) ||
                supplier.TaxNumber.Contains(term));
        }

        return new SupplierListViewModel
        {
            Suppliers = query
                .OrderBy(supplier => supplier.DisplayName)
                .Select(supplier => ToListItem(supplier))
                .ToList(),
            Form = new SupplierFormViewModel(),
            LegalTypeOptions = GetLegalTypeOptions(),
            SelectedLegalType = legalType,
            SearchTerm = searchTerm
        };
    }

    public SupplierListItemViewModel? GetById(Guid id)
    {
        var supplier = _dbContext.Suppliers
            .AsNoTracking()
            .FirstOrDefault(supplier => supplier.Id == id && !supplier.IsDeleted);

        return supplier is null ? null : ToListItem(supplier);
    }

    public void Add(SupplierFormViewModel form)
    {
        var legalTypeText = GetLegalTypeText(form.LegalType);
        var displayName = string.IsNullOrWhiteSpace(form.DisplayName)
            ? $"Yeni {legalTypeText}"
            : Clean(form.DisplayName);

        _dbContext.Suppliers.Add(new SupplierRecord
        {
            Id = Guid.NewGuid(),
            Code = GetNextSupplierCode(),
            DisplayName = displayName,
            LegalType = form.LegalType,
            TaxNumber = Clean(form.TaxNumber),
            TaxOffice = Clean(form.TaxOffice),
            ContactPerson = Clean(form.ContactPerson),
            ContactTitle = Clean(form.ContactTitle),
            Email = Clean(form.Email),
            Phone = Clean(form.Phone),
            City = Clean(form.City),
            Address = Clean(form.Address),
            CurrencyCode = NormalizeCurrencyCode(form.CurrencyCode),
            Balance = 0,
            PurchaseLimit = form.PurchaseLimit,
            PaymentTermDays = form.PaymentTermDays,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        });

        _dbContext.SaveChanges();
    }

    public bool Update(Guid id, SupplierFormViewModel form)
    {
        var supplier = _dbContext.Suppliers.FirstOrDefault(supplier => supplier.Id == id && !supplier.IsDeleted);

        if (supplier is null)
        {
            return false;
        }

        supplier.DisplayName = string.IsNullOrWhiteSpace(form.DisplayName)
            ? supplier.DisplayName
            : Clean(form.DisplayName);
        supplier.LegalType = form.LegalType;
        supplier.TaxNumber = Clean(form.TaxNumber);
        supplier.TaxOffice = Clean(form.TaxOffice);
        supplier.ContactPerson = Clean(form.ContactPerson);
        supplier.ContactTitle = Clean(form.ContactTitle);
        supplier.Email = Clean(form.Email);
        supplier.Phone = Clean(form.Phone);
        supplier.City = Clean(form.City);
        supplier.Address = Clean(form.Address);
        supplier.CurrencyCode = NormalizeCurrencyCode(form.CurrencyCode);
        supplier.PurchaseLimit = form.PurchaseLimit;
        supplier.PaymentTermDays = form.PaymentTermDays;
        supplier.UpdatedAt = DateTime.UtcNow;

        _dbContext.SaveChanges();
        return true;
    }

    public bool Delete(Guid id)
    {
        var supplier = _dbContext.Suppliers.FirstOrDefault(supplier => supplier.Id == id && !supplier.IsDeleted);

        if (supplier is null)
        {
            return false;
        }

        supplier.IsDeleted = true;
        supplier.UpdatedAt = DateTime.UtcNow;
        _dbContext.SaveChanges();
        return true;
    }

    private string GetNextSupplierCode()
    {
        var count = _dbContext.Suppliers.Count() + 1;
        string code;

        do
        {
            code = $"TED-{count:0000}";
            count++;
        }
        while (_dbContext.Suppliers.Any(supplier => supplier.Code == code));

        return code;
    }

    private static SupplierListItemViewModel ToListItem(SupplierRecord supplier)
    {
        return new SupplierListItemViewModel(
            Id: supplier.Id,
            Code: supplier.Code,
            DisplayName: supplier.DisplayName,
            LegalType: supplier.LegalType,
            LegalTypeText: GetLegalTypeText(supplier.LegalType),
            TaxNumber: supplier.TaxNumber,
            TaxOffice: supplier.TaxOffice,
            ContactPerson: supplier.ContactPerson,
            ContactTitle: supplier.ContactTitle,
            Email: supplier.Email,
            Phone: supplier.Phone,
            City: supplier.City,
            CurrencyCode: supplier.CurrencyCode,
            Balance: supplier.Balance,
            PurchaseLimit: supplier.PurchaseLimit,
            IsActive: supplier.IsActive);
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
