using NovacareERP.Application.CashManagement;

namespace NovacareERP.Infrastructure.Persistence;

public sealed class InMemoryCashManagementDirectoryService : ICashManagementDirectoryService
{
    private readonly List<CashAccountCardViewModel> _accounts =
    [
        new() { Name = "TL Kasa", AccountType = "Cash", CurrencyCode = "TL", Balance = 42500m, OwnerName = "Merkez Ofis" },
        new() { Name = "Saha Kasa", AccountType = "Cash", CurrencyCode = "TL", Balance = 8750m, OwnerName = "Operasyon" },
        new() { Name = "Banka TL Hesabi", AccountType = "Bank", CurrencyCode = "TL", Balance = 182450m, OwnerName = "Finans", IsIntegrated = true },
        new() { Name = "Banka USD Hesabi", AccountType = "Bank", CurrencyCode = "USD", Balance = 12400m, OwnerName = "Finans", IsIntegrated = true },
        new() { Name = "Banka EUR Hesabi", AccountType = "Bank", CurrencyCode = "EUR", Balance = 8300m, OwnerName = "Finans", IsIntegrated = true },
        new() { Name = "POS Hesabi", AccountType = "Pos", CurrencyCode = "TL", Balance = 15680m, OwnerName = "Satis", IsIntegrated = true },
        new() { Name = "Kredi Kartim", AccountType = "CreditCard", CurrencyCode = "TL", Balance = -9200m, OwnerName = "Yonetim" },
        new() { Name = "Sirket Ortagi Hesabi", AccountType = "Partner", CurrencyCode = "TL", Balance = 0m, OwnerName = "Yonetim" },
        new() { Name = "Veresiye Hesabi", AccountType = "Receivable", CurrencyCode = "TL", Balance = 12450m, OwnerName = "Satis" }
    ];

    public CashManagementWorkspaceViewModel GetWorkspace()
    {
        return new CashManagementWorkspaceViewModel
        {
            AccountGroups =
            [
                new()
                {
                    Title = "Kasa Hesaplari",
                    Description = "Gunluk nakit ve merkez kasa bakiyeleri",
                    IconName = "bi-cash-stack",
                    Accounts = _accounts.Where(account => account.AccountType == "Cash").ToList()
                },
                new()
                {
                    Title = "Banka Hesaplari",
                    Description = "Entegrasyonlu vadesiz ve doviz hesaplari",
                    IconName = "bi-bank2",
                    Accounts = _accounts.Where(account => account.AccountType == "Bank").ToList()
                },
                new()
                {
                    Title = "POS ve Kredi Karti",
                    Description = "POS tahsilatlari ve kredi karti odemeleri",
                    IconName = "bi-credit-card-2-front",
                    Accounts = _accounts.Where(account => account.AccountType is "Pos" or "CreditCard").ToList()
                },
                new()
                {
                    Title = "Ortak ve Veresiye",
                    Description = "Sirket ortagi ve veresiye takip hesaplari",
                    IconName = "bi-people-fill",
                    Accounts = _accounts.Where(account => account.AccountType is "Partner" or "Receivable").ToList()
                }
            ],
            RecentMovements =
            [
                new() { Date = new DateOnly(2026, 5, 31), AccountName = "Banka TL Hesabi", Description = "Nova Medikal tahsilati", Amount = 22500m, StatusText = "Eslesmis" },
                new() { Date = new DateOnly(2026, 5, 30), AccountName = "TL Kasa", Description = "Gun sonu kasa aktarimi", Amount = 8400m, StatusText = "Kayitli" },
                new() { Date = new DateOnly(2026, 5, 29), AccountName = "Kredi Kartim", Description = "Ofis gideri odemesi", Amount = -3250m, StatusText = "Kontrol" },
                new() { Date = new DateOnly(2026, 5, 28), AccountName = "POS Hesabi", Description = "Kartli satis tahsilati", Amount = 12680m, StatusText = "Bekliyor" }
            ],
            EmployeeSummaries =
            [
                new() { EmployeeName = "Emre Sutuna", RoleName = "Yonetici", AdvanceBalance = 0m, ExpenseTotal = 4200m },
                new() { EmployeeName = "Finans Ekibi", RoleName = "Operasyon", AdvanceBalance = 2500m, ExpenseTotal = 11850m }
            ]
        };
    }

    public void AddAccount(CashAccountFormViewModel form)
    {
        _accounts.Add(new CashAccountCardViewModel
        {
            Name = string.IsNullOrWhiteSpace(form.Name) ? "Yeni Hesap" : form.Name.Trim(),
            AccountType = string.IsNullOrWhiteSpace(form.AccountType) ? "Cash" : form.AccountType.Trim(),
            CurrencyCode = string.IsNullOrWhiteSpace(form.CurrencyCode) ? "TL" : form.CurrencyCode.Trim().ToUpperInvariant(),
            Balance = form.OpeningBalance,
            OwnerName = form.OwnerName?.Trim() ?? "",
            IsIntegrated = form.IsIntegrated
        });
    }
}
