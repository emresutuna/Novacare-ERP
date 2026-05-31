using NovacareERP.Domain.Enums;

namespace NovacareERP.Infrastructure.Persistence.Entities;

public sealed class SupplierRecord
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Code { get; set; } = "";
    public string DisplayName { get; set; } = "";
    public CustomerLegalType LegalType { get; set; }
    public string TaxNumber { get; set; } = "";
    public string TaxOffice { get; set; } = "";
    public string ContactPerson { get; set; } = "";
    public string ContactTitle { get; set; } = "";
    public string Email { get; set; } = "";
    public string Phone { get; set; } = "";
    public string City { get; set; } = "";
    public string Address { get; set; } = "";
    public string CurrencyCode { get; set; } = "TRY";
    public decimal Balance { get; set; }
    public decimal PurchaseLimit { get; set; }
    public int PaymentTermDays { get; set; } = 30;
    public bool IsActive { get; set; } = true;
    public bool IsDeleted { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}
