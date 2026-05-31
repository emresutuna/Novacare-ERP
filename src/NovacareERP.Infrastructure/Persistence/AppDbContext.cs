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
    public DbSet<SupplierRecord> Suppliers => Set<SupplierRecord>();
    public DbSet<AppointmentRecord> Appointments => Set<AppointmentRecord>();
    public DbSet<InvoiceRecord> Invoices => Set<InvoiceRecord>();
    public DbSet<InvoiceLineRecord> InvoiceLines => Set<InvoiceLineRecord>();
    public DbSet<CashAccountRecord> CashAccounts => Set<CashAccountRecord>();
    public DbSet<CashMovementRecord> CashMovements => Set<CashMovementRecord>();
    public DbSet<EmployeeCashSummaryRecord> EmployeeCashSummaries => Set<EmployeeCashSummaryRecord>();

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

        modelBuilder.Entity<SupplierRecord>(entity =>
        {
            entity.ToTable("Suppliers");
            entity.HasKey(supplier => supplier.Id);
            entity.HasIndex(supplier => supplier.Code).IsUnique();
            entity.HasIndex(supplier => supplier.DisplayName);
            entity.Property(supplier => supplier.Code).HasMaxLength(32).IsRequired();
            entity.Property(supplier => supplier.DisplayName).HasMaxLength(220).IsRequired();
            entity.Property(supplier => supplier.TaxNumber).HasMaxLength(32);
            entity.Property(supplier => supplier.TaxOffice).HasMaxLength(120);
            entity.Property(supplier => supplier.ContactPerson).HasMaxLength(160);
            entity.Property(supplier => supplier.ContactTitle).HasMaxLength(120);
            entity.Property(supplier => supplier.Email).HasMaxLength(240);
            entity.Property(supplier => supplier.Phone).HasMaxLength(64);
            entity.Property(supplier => supplier.City).HasMaxLength(80);
            entity.Property(supplier => supplier.Address).HasMaxLength(500);
            entity.Property(supplier => supplier.CurrencyCode).HasMaxLength(3).IsRequired();
            entity.Property(supplier => supplier.Balance).HasPrecision(18, 2);
            entity.Property(supplier => supplier.PurchaseLimit).HasPrecision(18, 2);
        });

        modelBuilder.Entity<InvoiceRecord>(entity =>
        {
            entity.ToTable("Invoices");
            entity.HasKey(invoice => invoice.Id);
            entity.HasIndex(invoice => invoice.InvoiceType);
            entity.HasIndex(invoice => invoice.CustomerId);
            entity.HasIndex(invoice => invoice.SupplierId);
            entity.HasIndex(invoice => invoice.InvoiceNumber);
            entity.HasIndex(invoice => invoice.IssueDate);
            entity.Property(invoice => invoice.PartyName).HasMaxLength(220).IsRequired();
            entity.Property(invoice => invoice.PartyTaxNumber).HasMaxLength(32);
            entity.Property(invoice => invoice.InvoiceNumber).HasMaxLength(40).IsRequired();
            entity.Property(invoice => invoice.CurrencyCode).HasMaxLength(3).IsRequired();
            entity.Property(invoice => invoice.ExchangeRate).HasPrecision(18, 6);
            entity.Property(invoice => invoice.Description).HasMaxLength(1000);
            entity.Property(invoice => invoice.DocumentUuid).HasMaxLength(64);
            entity.Property(invoice => invoice.Ettn).HasMaxLength(64);
            entity.HasMany(invoice => invoice.Items)
                .WithOne(line => line.Invoice)
                .HasForeignKey(line => line.InvoiceId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<InvoiceLineRecord>(entity =>
        {
            entity.ToTable("InvoiceLines");
            entity.HasKey(line => line.Id);
            entity.HasIndex(line => line.InvoiceId);
            entity.Property(line => line.ProductOrServiceName).HasMaxLength(220).IsRequired();
            entity.Property(line => line.Quantity).HasPrecision(18, 4);
            entity.Property(line => line.UnitPrice).HasPrecision(18, 2);
            entity.Property(line => line.TaxRate).HasPrecision(5, 2);
            entity.Property(line => line.TotalAmount).HasPrecision(18, 2);
        });

        modelBuilder.Entity<CashAccountRecord>(entity =>
        {
            entity.ToTable("CashAccounts");
            entity.HasKey(account => account.Id);
            entity.HasIndex(account => account.AccountType);
            entity.HasIndex(account => account.Name);
            entity.Property(account => account.AccountType).HasMaxLength(32).IsRequired();
            entity.Property(account => account.Name).HasMaxLength(160).IsRequired();
            entity.Property(account => account.CurrencyCode).HasMaxLength(3).IsRequired();
            entity.Property(account => account.OpeningBalance).HasPrecision(18, 2);
            entity.Property(account => account.CurrentBalance).HasPrecision(18, 2);
            entity.Property(account => account.OwnerName).HasMaxLength(160);
            entity.HasMany(account => account.Movements)
                .WithOne(movement => movement.CashAccount)
                .HasForeignKey(movement => movement.CashAccountId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<CashMovementRecord>(entity =>
        {
            entity.ToTable("CashMovements");
            entity.HasKey(movement => movement.Id);
            entity.HasIndex(movement => movement.CashAccountId);
            entity.HasIndex(movement => movement.Date);
            entity.Property(movement => movement.Description).HasMaxLength(320).IsRequired();
            entity.Property(movement => movement.Amount).HasPrecision(18, 2);
            entity.Property(movement => movement.CurrencyCode).HasMaxLength(3).IsRequired();
            entity.Property(movement => movement.StatusText).HasMaxLength(40).IsRequired();
        });

        modelBuilder.Entity<EmployeeCashSummaryRecord>(entity =>
        {
            entity.ToTable("EmployeeCashSummaries");
            entity.HasKey(employee => employee.Id);
            entity.HasIndex(employee => employee.EmployeeName);
            entity.Property(employee => employee.EmployeeName).HasMaxLength(160).IsRequired();
            entity.Property(employee => employee.RoleName).HasMaxLength(120);
            entity.Property(employee => employee.AdvanceBalance).HasPrecision(18, 2);
            entity.Property(employee => employee.ExpenseTotal).HasPrecision(18, 2);
        });
    }
}
