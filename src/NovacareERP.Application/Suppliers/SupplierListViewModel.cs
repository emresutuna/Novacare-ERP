using NovacareERP.Application.Customers;
using NovacareERP.Domain.Enums;

namespace NovacareERP.Application.Suppliers;

public sealed class SupplierListViewModel
{
    public IReadOnlyList<SupplierListItemViewModel> Suppliers { get; init; } = [];
    public SupplierFormViewModel Form { get; init; } = new();
    public IReadOnlyList<CustomerLegalTypeOption> LegalTypeOptions { get; init; } = [];
    public CustomerLegalType? SelectedLegalType { get; init; }
    public string? SearchTerm { get; init; }
}
