using Microsoft.EntityFrameworkCore;
using NovacareERP.Application.Customers;
using NovacareERP.Domain.Enums;
using NovacareERP.Infrastructure.Persistence.Entities;

namespace NovacareERP.Infrastructure.Persistence;

public sealed class EfCustomerDirectoryService : ICustomerDirectoryService
{
    private readonly AppDbContext _dbContext;

    public EfCustomerDirectoryService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public CustomerListViewModel GetList(CustomerLegalType? legalType, string? searchTerm)
    {
        var query = _dbContext.Customers
            .AsNoTracking()
            .Where(customer => !customer.IsDeleted);

        if (legalType.HasValue)
        {
            query = query.Where(customer => customer.LegalType == legalType.Value);
        }

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.Trim();
            query = query.Where(customer =>
                customer.DisplayName.Contains(term) ||
                customer.Code.Contains(term) ||
                customer.ContactPerson.Contains(term) ||
                customer.TaxNumber.Contains(term));
        }

        return new CustomerListViewModel
        {
            Customers = query
                .OrderBy(customer => customer.DisplayName)
                .Select(customer => ToListItem(customer))
                .ToList(),
            Form = new CustomerFormViewModel(),
            LegalTypeOptions = GetLegalTypeOptions(),
            SelectedLegalType = legalType,
            SearchTerm = searchTerm
        };
    }

    public CustomerListItemViewModel? GetById(Guid id)
    {
        var customer = _dbContext.Customers
            .AsNoTracking()
            .FirstOrDefault(customer => customer.Id == id && !customer.IsDeleted);

        return customer is null ? null : ToListItem(customer);
    }

    public void Add(CustomerFormViewModel form)
    {
        var legalTypeText = GetLegalTypeText(form.LegalType);
        var displayName = string.IsNullOrWhiteSpace(form.DisplayName)
            ? $"Yeni {legalTypeText}"
            : Clean(form.DisplayName);

        _dbContext.Customers.Add(new CustomerRecord
        {
            Id = Guid.NewGuid(),
            Code = GetNextCustomerCode(),
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
            CreditLimit = form.CreditLimit,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        });

        _dbContext.SaveChanges();
    }

    public bool Update(Guid id, CustomerFormViewModel form)
    {
        var customer = _dbContext.Customers.FirstOrDefault(customer => customer.Id == id && !customer.IsDeleted);

        if (customer is null)
        {
            return false;
        }

        var legalTypeText = GetLegalTypeText(form.LegalType);
        customer.DisplayName = string.IsNullOrWhiteSpace(form.DisplayName)
            ? customer.DisplayName
            : Clean(form.DisplayName);
        customer.LegalType = form.LegalType;
        customer.TaxNumber = Clean(form.TaxNumber);
        customer.TaxOffice = Clean(form.TaxOffice);
        customer.ContactPerson = Clean(form.ContactPerson);
        customer.ContactTitle = Clean(form.ContactTitle);
        customer.Email = Clean(form.Email);
        customer.Phone = Clean(form.Phone);
        customer.City = Clean(form.City);
        customer.Address = Clean(form.Address);
        customer.CurrencyCode = NormalizeCurrencyCode(form.CurrencyCode);
        customer.CreditLimit = form.CreditLimit;
        customer.UpdatedAt = DateTime.UtcNow;

        _dbContext.SaveChanges();
        return true;
    }

    public bool Delete(Guid id)
    {
        var customer = _dbContext.Customers.FirstOrDefault(customer => customer.Id == id && !customer.IsDeleted);

        if (customer is null)
        {
            return false;
        }

        customer.IsDeleted = true;
        customer.UpdatedAt = DateTime.UtcNow;
        _dbContext.SaveChanges();
        return true;
    }

    private string GetNextCustomerCode()
    {
        var count = _dbContext.Customers.Count() + 1;
        string code;

        do
        {
            code = $"MUS-{count:0000}";
            count++;
        }
        while (_dbContext.Customers.Any(customer => customer.Code == code));

        return code;
    }

    private static CustomerListItemViewModel ToListItem(CustomerRecord customer)
    {
        return new CustomerListItemViewModel(
            Id: customer.Id,
            Code: customer.Code,
            DisplayName: customer.DisplayName,
            LegalType: customer.LegalType,
            LegalTypeText: GetLegalTypeText(customer.LegalType),
            TaxNumber: customer.TaxNumber,
            TaxOffice: customer.TaxOffice,
            ContactPerson: customer.ContactPerson,
            ContactTitle: customer.ContactTitle,
            Email: customer.Email,
            Phone: customer.Phone,
            City: customer.City,
            CurrencyCode: customer.CurrencyCode,
            Balance: customer.Balance,
            CreditLimit: customer.CreditLimit,
            IsActive: customer.IsActive);
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
