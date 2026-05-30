using NovacareERP.Domain.Enums;

namespace NovacareERP.Application.Customers;

public sealed record CustomerListItemViewModel(
    Guid Id,
    string Code,
    string DisplayName,
    CustomerLegalType LegalType,
    string LegalTypeText,
    string TaxNumber,
    string TaxOffice,
    string ContactPerson,
    string ContactTitle,
    string Email,
    string Phone,
    string City,
    string CurrencyCode,
    decimal Balance,
    decimal CreditLimit,
    bool IsActive);
