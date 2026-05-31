namespace NovacareERP.Application.CashManagement;

public sealed class CashManagementWorkspaceViewModel
{
    public IReadOnlyList<CashAccountGroupViewModel> AccountGroups { get; init; } = [];
    public IReadOnlyList<CashMovementViewModel> RecentMovements { get; init; } = [];
    public IReadOnlyList<EmployeeCashSummaryViewModel> EmployeeSummaries { get; init; } = [];
    public CashAccountFormViewModel Form { get; init; } = new();
    public decimal TotalBalance => AccountGroups.SelectMany(group => group.Accounts).Sum(account => account.Balance);
    public int AccountCount => AccountGroups.Sum(group => group.Accounts.Count);
    public int IntegrationCount => AccountGroups.SelectMany(group => group.Accounts).Count(account => account.IsIntegrated);
}

public sealed class CashAccountGroupViewModel
{
    public string Title { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string IconName { get; init; } = "bi-wallet2";
    public IReadOnlyList<CashAccountCardViewModel> Accounts { get; init; } = [];
}

public sealed class CashAccountCardViewModel
{
    public string Name { get; init; } = string.Empty;
    public string AccountType { get; init; } = "Cash";
    public string CurrencyCode { get; init; } = "TL";
    public decimal Balance { get; init; }
    public string OwnerName { get; init; } = string.Empty;
    public bool IsIntegrated { get; init; }
}

public sealed class CashAccountFormViewModel
{
    public string AccountType { get; init; } = "Cash";
    public string Name { get; init; } = string.Empty;
    public string CurrencyCode { get; init; } = "TL";
    public decimal OpeningBalance { get; init; }
    public string OwnerName { get; init; } = string.Empty;
    public bool IsIntegrated { get; init; }
}

public sealed class CashMovementViewModel
{
    public DateOnly Date { get; init; }
    public string AccountName { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public decimal Amount { get; init; }
    public string CurrencyCode { get; init; } = "TL";
    public string StatusText { get; init; } = string.Empty;
}

public sealed class EmployeeCashSummaryViewModel
{
    public string EmployeeName { get; init; } = string.Empty;
    public string RoleName { get; init; } = string.Empty;
    public decimal AdvanceBalance { get; init; }
    public decimal ExpenseTotal { get; init; }
}
