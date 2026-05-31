using Microsoft.EntityFrameworkCore;
using NovacareERP.Application.CashManagement;
using NovacareERP.Infrastructure.Persistence.Entities;

namespace NovacareERP.Infrastructure.Persistence;

public sealed class EfCashManagementDirectoryService : ICashManagementDirectoryService
{
    private readonly AppDbContext _dbContext;

    public EfCashManagementDirectoryService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public CashManagementWorkspaceViewModel GetWorkspace()
    {
        var accounts = _dbContext.CashAccounts
            .AsNoTracking()
            .Where(account => !account.IsDeleted)
            .OrderBy(account => account.AccountType)
            .ThenBy(account => account.Name)
            .ToList();

        var movements = _dbContext.CashMovements
            .AsNoTracking()
            .Include(movement => movement.CashAccount)
            .Where(movement => !movement.IsDeleted && movement.CashAccount != null && !movement.CashAccount.IsDeleted)
            .OrderByDescending(movement => movement.Date)
            .ThenByDescending(movement => movement.CreatedAt)
            .Take(8)
            .Select(ToMovementViewModel)
            .ToList();

        var employees = _dbContext.EmployeeCashSummaries
            .AsNoTracking()
            .Where(employee => !employee.IsDeleted)
            .OrderBy(employee => employee.EmployeeName)
            .Select(ToEmployeeViewModel)
            .ToList();

        return new CashManagementWorkspaceViewModel
        {
            AccountGroups = BuildGroups(accounts),
            RecentMovements = movements,
            EmployeeSummaries = employees
        };
    }

    public void AddAccount(CashAccountFormViewModel form)
    {
        var currencyCode = NormalizeCurrencyCode(form.CurrencyCode);
        var name = string.IsNullOrWhiteSpace(form.Name)
            ? $"{GetAccountTypeTitle(form.AccountType)} Hesabi"
            : Clean(form.Name);

        var account = new CashAccountRecord
        {
            Id = Guid.NewGuid(),
            AccountType = NormalizeAccountType(form.AccountType),
            Name = name,
            CurrencyCode = currencyCode,
            OpeningBalance = form.OpeningBalance,
            CurrentBalance = form.OpeningBalance,
            OwnerName = Clean(form.OwnerName),
            IsIntegrated = form.IsIntegrated,
            CreatedAt = DateTime.UtcNow
        };

        _dbContext.CashAccounts.Add(account);

        if (form.OpeningBalance != 0)
        {
            _dbContext.CashMovements.Add(new CashMovementRecord
            {
                Id = Guid.NewGuid(),
                CashAccountId = account.Id,
                Date = DateOnly.FromDateTime(DateTime.Today),
                Description = "Acilis bakiyesi",
                Amount = form.OpeningBalance,
                CurrencyCode = currencyCode,
                StatusText = "Kayitli",
                CreatedAt = DateTime.UtcNow
            });
        }

        _dbContext.SaveChanges();
    }

    private static IReadOnlyList<CashAccountGroupViewModel> BuildGroups(IReadOnlyList<CashAccountRecord> accounts)
    {
        return
        [
            CreateGroup(accounts, "Cash", "Kasa Hesaplari", "Gunluk nakit ve merkez kasa bakiyeleri", "bi-cash-stack"),
            CreateGroup(accounts, "Bank", "Banka Hesaplari", "Entegrasyonlu vadesiz ve doviz hesaplari", "bi-bank2"),
            CreateGroup(accounts, "Pos", "POS ve Kredi Karti", "POS tahsilatlari ve kredi karti odemeleri", "bi-credit-card-2-front", ["CreditCard"]),
            CreateGroup(accounts, "Partner", "Ortak ve Veresiye", "Sirket ortagi ve veresiye takip hesaplari", "bi-people-fill", ["Receivable"])
        ];
    }

    private static CashAccountGroupViewModel CreateGroup(
        IReadOnlyList<CashAccountRecord> accounts,
        string accountType,
        string title,
        string description,
        string iconName,
        IReadOnlyList<string>? extraTypes = null)
    {
        var types = new[] { accountType }.Concat(extraTypes ?? []).ToHashSet(StringComparer.OrdinalIgnoreCase);

        return new CashAccountGroupViewModel
        {
            Title = title,
            Description = description,
            IconName = iconName,
            Accounts = accounts
                .Where(account => types.Contains(account.AccountType))
                .Select(ToAccountCard)
                .ToList()
        };
    }

    private static CashAccountCardViewModel ToAccountCard(CashAccountRecord account) =>
        new()
        {
            Name = account.Name,
            AccountType = account.AccountType,
            CurrencyCode = account.CurrencyCode,
            Balance = account.CurrentBalance,
            OwnerName = account.OwnerName,
            IsIntegrated = account.IsIntegrated
        };

    private static CashMovementViewModel ToMovementViewModel(CashMovementRecord movement) =>
        new()
        {
            Date = movement.Date,
            AccountName = movement.CashAccount?.Name ?? "",
            Description = movement.Description,
            Amount = movement.Amount,
            CurrencyCode = movement.CurrencyCode,
            StatusText = movement.StatusText
        };

    private static EmployeeCashSummaryViewModel ToEmployeeViewModel(EmployeeCashSummaryRecord employee) =>
        new()
        {
            EmployeeName = employee.EmployeeName,
            RoleName = employee.RoleName,
            AdvanceBalance = employee.AdvanceBalance,
            ExpenseTotal = employee.ExpenseTotal
        };

    private static string GetAccountTypeTitle(string? value) =>
        NormalizeAccountType(value) switch
        {
            "Bank" => "Banka",
            "Pos" => "POS",
            "CreditCard" => "Kredi Karti",
            "Partner" => "Ortak",
            "Receivable" => "Veresiye",
            _ => "Kasa"
        };

    private static string NormalizeAccountType(string? value)
    {
        var accountType = Clean(value);
        return accountType is "Cash" or "Bank" or "Pos" or "CreditCard" or "Partner" or "Receivable"
            ? accountType
            : "Cash";
    }

    private static string NormalizeCurrencyCode(string? value)
    {
        var currencyCode = Clean(value).ToUpperInvariant();
        return currencyCode is "TRY" or "TL" ? "TL" : currencyCode is "USD" or "EUR" ? currencyCode : "TL";
    }

    private static string Clean(string? value) => value?.Trim() ?? "";
}
