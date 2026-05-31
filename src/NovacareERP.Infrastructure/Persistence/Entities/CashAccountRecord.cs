namespace NovacareERP.Infrastructure.Persistence.Entities;

public sealed class CashAccountRecord
{
    public Guid Id { get; set; }
    public string AccountType { get; set; } = "Cash";
    public string Name { get; set; } = string.Empty;
    public string CurrencyCode { get; set; } = "TRY";
    public decimal OpeningBalance { get; set; }
    public decimal CurrentBalance { get; set; }
    public string OwnerName { get; set; } = string.Empty;
    public bool IsIntegrated { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public List<CashMovementRecord> Movements { get; set; } = [];
}
