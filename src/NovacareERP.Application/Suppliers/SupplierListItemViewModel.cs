using NovacareERP.Domain.Enums;

namespace NovacareERP.Application.Suppliers;

public sealed record SupplierListItemViewModel(
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
    decimal PurchaseLimit,
    bool IsActive);
