using Microsoft.EntityFrameworkCore;
using NovacareERP.Infrastructure.Persistence.Entities;

namespace NovacareERP.Infrastructure.Persistence;

public sealed class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<CustomerRecord> Customers => Set<CustomerRecord>();
    public DbSet<AppointmentRecord> Appointments => Set<AppointmentRecord>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CustomerRecord>(entity =>
        {
            entity.ToTable("Customers");
            entity.HasKey(customer => customer.Id);
            entity.HasIndex(customer => customer.Code).IsUnique();
            entity.HasIndex(customer => customer.DisplayName);
            entity.Property(customer => customer.Code).HasMaxLength(32).IsRequired();
            entity.Property(customer => customer.DisplayName).HasMaxLength(220).IsRequired();
            entity.Property(customer => customer.TaxNumber).HasMaxLength(32);
            entity.Property(customer => customer.TaxOffice).HasMaxLength(120);
            entity.Property(customer => customer.ContactPerson).HasMaxLength(160);
            entity.Property(customer => customer.ContactTitle).HasMaxLength(120);
            entity.Property(customer => customer.Email).HasMaxLength(240);
            entity.Property(customer => customer.Phone).HasMaxLength(64);
            entity.Property(customer => customer.City).HasMaxLength(80);
            entity.Property(customer => customer.Address).HasMaxLength(500);
            entity.Property(customer => customer.CurrencyCode).HasMaxLength(3).IsRequired();
            entity.Property(customer => customer.Balance).HasPrecision(18, 2);
            entity.Property(customer => customer.CreditLimit).HasPrecision(18, 2);
        });

        modelBuilder.Entity<AppointmentRecord>(entity =>
        {
            entity.ToTable("Appointments");
            entity.HasKey(appointment => appointment.Id);
            entity.HasIndex(appointment => appointment.CustomerId);
            entity.HasIndex(appointment => new { appointment.Date, appointment.Time });
            entity.Property(appointment => appointment.CustomerName).HasMaxLength(160);
            entity.Property(appointment => appointment.CompanyName).HasMaxLength(220).IsRequired();
            entity.Property(appointment => appointment.Title).HasMaxLength(180).IsRequired();
            entity.Property(appointment => appointment.ContactPerson).HasMaxLength(160);
            entity.Property(appointment => appointment.CustomerDemand).HasMaxLength(1000);
            entity.Property(appointment => appointment.Notes).HasMaxLength(1000);
        });
    }
}
