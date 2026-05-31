using Microsoft.EntityFrameworkCore;
using NovacareERP.Domain.Enums;
using NovacareERP.Infrastructure.Persistence.Entities;

namespace NovacareERP.Infrastructure.Persistence;

public static class DatabaseSeeder
{
    public static void EnsureSeedData(AppDbContext dbContext)
    {
        dbContext.Database.Migrate();

        if (!dbContext.Customers.Any())
        {
            dbContext.Customers.AddRange(
                new CustomerRecord
                {
                    Id = Guid.Parse("5b6c6cdd-3ac8-4ad3-8e72-bac1f31dd001"),
                    Code = "MUS-0001",
                    DisplayName = "Nova Medikal A.S.",
                    LegalType = CustomerLegalType.JointStockCompany,
                    TaxNumber = "4890123456",
                    TaxOffice = "Kadikoy",
                    ContactPerson = "Derya Yilmaz",
                    ContactTitle = "Finans Sorumlusu",
                    Email = "finans@novamedikal.com",
                    Phone = "+90 216 000 10 20",
                    City = "Istanbul",
                    CurrencyCode = "TRY",
                    Balance = 22500,
                    CreditLimit = 150000
                },
                new CustomerRecord
                {
                    Id = Guid.Parse("5b6c6cdd-3ac8-4ad3-8e72-bac1f31dd002"),
                    Code = "MUS-0002",
                    DisplayName = "Ayse Demir",
                    LegalType = CustomerLegalType.EndUser,
                    TaxNumber = "11111111111",
                    ContactPerson = "Ayse Demir",
                    ContactTitle = "Son Kullanici",
                    Email = "ayse.demir@example.com",
                    Phone = "+90 555 111 22 33",
                    City = "Ankara",
                    CurrencyCode = "TRY",
                    Balance = 8400,
                    CreditLimit = 25000
                },
                new CustomerRecord
                {
                    Id = Guid.Parse("5b6c6cdd-3ac8-4ad3-8e72-bac1f31dd003"),
                    Code = "MUS-0003",
                    DisplayName = "Sutuna Danismanlik",
                    LegalType = CustomerLegalType.SoleProprietorship,
                    TaxNumber = "12345678901",
                    TaxOffice = "Besiktas",
                    ContactPerson = "Emre Sutuna",
                    ContactTitle = "Firma Yetkilisi",
                    Email = "emre@sutunadanismanlik.com",
                    Phone = "+90 532 222 44 55",
                    City = "Istanbul",
                    CurrencyCode = "EUR",
                    CreditLimit = 50000
                });

            dbContext.SaveChanges();
        }

        if (!dbContext.Appointments.Any())
        {
            var customer = dbContext.Customers
                .OrderBy(customer => customer.DisplayName)
                .FirstOrDefault();

            if (customer is null)
            {
                return;
            }

            dbContext.Appointments.Add(new AppointmentRecord
            {
                Id = Guid.Parse("9a6c6cdd-3ac8-4ad3-8e72-bac1f31dd001"),
                CustomerId = customer.Id,
                CustomerName = string.IsNullOrWhiteSpace(customer.ContactPerson) ? customer.DisplayName : customer.ContactPerson,
                CompanyName = customer.DisplayName,
                Title = "Ilk Tanisma ve Urun Sunumu",
                ContactPerson = customer.ContactPerson,
                Date = DateOnly.FromDateTime(DateTime.Today),
                Time = new TimeOnly(14, 30),
                CustomerDemand = "ERP entegrasyonu ve teklif modulleri hakkinda detayli bilgi istiyor.",
                Notes = "Gorusme olumlu gecti. Teklif hazirlanacak.",
                CreatedAt = DateTime.UtcNow.AddHours(-2)
            });

            dbContext.SaveChanges();
        }

        if (!dbContext.Suppliers.Any())
        {
            dbContext.Suppliers.AddRange(
                new SupplierRecord
                {
                    Id = Guid.Parse("6b6c6cdd-3ac8-4ad3-8e72-bac1f31dd001"),
                    Code = "TED-0001",
                    DisplayName = "Medline Tedarik A.S.",
                    LegalType = CustomerLegalType.JointStockCompany,
                    TaxNumber = "5820123456",
                    TaxOffice = "Umraniye",
                    ContactPerson = "Mert Kaya",
                    ContactTitle = "Satis Yoneticisi",
                    Email = "satis@medlinetedarik.com",
                    Phone = "+90 216 100 20 30",
                    City = "Istanbul",
                    CurrencyCode = "TRY",
                    Balance = 12800,
                    PurchaseLimit = 250000,
                    PaymentTermDays = 30
                },
                new SupplierRecord
                {
                    Id = Guid.Parse("6b6c6cdd-3ac8-4ad3-8e72-bac1f31dd002"),
                    Code = "TED-0002",
                    DisplayName = "Ankara Laboratuvar Urunleri",
                    LegalType = CustomerLegalType.LimitedCompany,
                    TaxNumber = "4820123456",
                    TaxOffice = "Cankaya",
                    ContactPerson = "Selin Acar",
                    ContactTitle = "Operasyon Sorumlusu",
                    Email = "operasyon@anklab.com",
                    Phone = "+90 312 400 50 60",
                    City = "Ankara",
                    CurrencyCode = "TRY",
                    PurchaseLimit = 175000,
                    PaymentTermDays = 45
                });

            dbContext.SaveChanges();
        }

        if (!dbContext.CashAccounts.Any())
        {
            var cashAccounts = new[]
            {
                CreateCashAccount("7b6c6cdd-3ac8-4ad3-8e72-bac1f31dd001", "Cash", "TL Kasa", "TL", 42500m, "Merkez Ofis", false),
                CreateCashAccount("7b6c6cdd-3ac8-4ad3-8e72-bac1f31dd002", "Cash", "Saha Kasa", "TL", 8750m, "Operasyon", false),
                CreateCashAccount("7b6c6cdd-3ac8-4ad3-8e72-bac1f31dd003", "Bank", "Banka TL Hesabi", "TL", 182450m, "Finans", true),
                CreateCashAccount("7b6c6cdd-3ac8-4ad3-8e72-bac1f31dd004", "Bank", "Banka USD Hesabi", "USD", 12400m, "Finans", true),
                CreateCashAccount("7b6c6cdd-3ac8-4ad3-8e72-bac1f31dd005", "Bank", "Banka EUR Hesabi", "EUR", 8300m, "Finans", true),
                CreateCashAccount("7b6c6cdd-3ac8-4ad3-8e72-bac1f31dd006", "Pos", "POS Hesabi", "TL", 15680m, "Satis", true),
                CreateCashAccount("7b6c6cdd-3ac8-4ad3-8e72-bac1f31dd007", "CreditCard", "Kredi Kartim", "TL", -9200m, "Yonetim", false),
                CreateCashAccount("7b6c6cdd-3ac8-4ad3-8e72-bac1f31dd008", "Partner", "Sirket Ortagi Hesabi", "TL", 0m, "Yonetim", false),
                CreateCashAccount("7b6c6cdd-3ac8-4ad3-8e72-bac1f31dd009", "Receivable", "Veresiye Hesabi", "TL", 12450m, "Satis", false)
            };

            dbContext.CashAccounts.AddRange(cashAccounts);
            dbContext.CashMovements.AddRange(
                CreateCashMovement("8b6c6cdd-3ac8-4ad3-8e72-bac1f31dd001", cashAccounts[2], DateTime.Today, "Nova Medikal tahsilati", 22500m, "Eslesmis"),
                CreateCashMovement("8b6c6cdd-3ac8-4ad3-8e72-bac1f31dd002", cashAccounts[0], DateTime.Today.AddDays(-1), "Gun sonu kasa aktarimi", 8400m, "Kayitli"),
                CreateCashMovement("8b6c6cdd-3ac8-4ad3-8e72-bac1f31dd003", cashAccounts[6], DateTime.Today.AddDays(-2), "Ofis gideri odemesi", -3250m, "Kontrol"),
                CreateCashMovement("8b6c6cdd-3ac8-4ad3-8e72-bac1f31dd004", cashAccounts[5], DateTime.Today.AddDays(-3), "Kartli satis tahsilati", 12680m, "Bekliyor"));

            dbContext.EmployeeCashSummaries.AddRange(
                new EmployeeCashSummaryRecord
                {
                    Id = Guid.Parse("9b6c6cdd-3ac8-4ad3-8e72-bac1f31dd001"),
                    EmployeeName = "Emre Sutuna",
                    RoleName = "Yonetici",
                    AdvanceBalance = 0,
                    ExpenseTotal = 4200,
                    CreatedAt = DateTime.UtcNow
                },
                new EmployeeCashSummaryRecord
                {
                    Id = Guid.Parse("9b6c6cdd-3ac8-4ad3-8e72-bac1f31dd002"),
                    EmployeeName = "Finans Ekibi",
                    RoleName = "Operasyon",
                    AdvanceBalance = 2500,
                    ExpenseTotal = 11850,
                    CreatedAt = DateTime.UtcNow
                });

            dbContext.SaveChanges();
        }

        var fallbackCustomer = dbContext.Customers
            .OrderBy(customer => customer.DisplayName)
            .FirstOrDefault();

        if (fallbackCustomer is null)
        {
            return;
        }

        var incompleteAppointments = dbContext.Appointments
            .Where(appointment =>
                string.IsNullOrWhiteSpace(appointment.CompanyName) ||
                string.IsNullOrWhiteSpace(appointment.CustomerName) ||
                string.IsNullOrWhiteSpace(appointment.ContactPerson) ||
                string.IsNullOrWhiteSpace(appointment.CustomerDemand) ||
                string.IsNullOrWhiteSpace(appointment.Notes))
            .ToList();

        foreach (var appointment in incompleteAppointments)
        {
            appointment.CustomerId = appointment.CustomerId == Guid.Empty ? fallbackCustomer.Id : appointment.CustomerId;
            appointment.CompanyName = string.IsNullOrWhiteSpace(appointment.CompanyName) ? fallbackCustomer.DisplayName : appointment.CompanyName;
            appointment.CustomerName = string.IsNullOrWhiteSpace(appointment.CustomerName)
                ? (string.IsNullOrWhiteSpace(fallbackCustomer.ContactPerson) ? fallbackCustomer.DisplayName : fallbackCustomer.ContactPerson)
                : appointment.CustomerName;
            appointment.ContactPerson = string.IsNullOrWhiteSpace(appointment.ContactPerson) ? "Emre Sutuna" : appointment.ContactPerson;
            appointment.CustomerDemand = string.IsNullOrWhiteSpace(appointment.CustomerDemand)
                ? "Musteri ile genel durum degerlendirmesi, ihtiyac analizi ve sonraki adimlar gorusulecek."
                : appointment.CustomerDemand;
            appointment.Notes = string.IsNullOrWhiteSpace(appointment.Notes)
                ? "Randevu sonrasi aksiyonlar ve takip notlari bu alanda guncellenecek."
                : appointment.Notes;
            appointment.UpdatedAt = DateTime.UtcNow;
        }

        if (incompleteAppointments.Count > 0)
        {
            dbContext.SaveChanges();
        }
    }

    private static CashAccountRecord CreateCashAccount(
        string id,
        string accountType,
        string name,
        string currencyCode,
        decimal balance,
        string ownerName,
        bool isIntegrated) =>
        new()
        {
            Id = Guid.Parse(id),
            AccountType = accountType,
            Name = name,
            CurrencyCode = currencyCode,
            OpeningBalance = balance,
            CurrentBalance = balance,
            OwnerName = ownerName,
            IsIntegrated = isIntegrated,
            CreatedAt = DateTime.UtcNow
        };

    private static CashMovementRecord CreateCashMovement(
        string id,
        CashAccountRecord account,
        DateTime date,
        string description,
        decimal amount,
        string statusText) =>
        new()
        {
            Id = Guid.Parse(id),
            CashAccountId = account.Id,
            Date = DateOnly.FromDateTime(date),
            Description = description,
            Amount = amount,
            CurrencyCode = account.CurrencyCode,
            StatusText = statusText,
            CreatedAt = DateTime.UtcNow
        };
}
