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
}
