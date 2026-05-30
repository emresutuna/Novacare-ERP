using NovacareERP.Domain.Enums;

namespace NovacareERP.Application.Customers;

public sealed class CustomerListViewModel
{
    public IReadOnlyList<CustomerListItemViewModel> Customers { get; init; } = [];
    public CustomerFormViewModel Form { get; init; } = new();
    public IReadOnlyList<CustomerLegalTypeOption> LegalTypeOptions { get; init; } = [];
    public CustomerLegalType? SelectedLegalType { get; init; }
    public string? SearchTerm { get; init; }
}
