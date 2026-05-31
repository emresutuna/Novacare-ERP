namespace NovacareERP.Infrastructure.Persistence.Entities;

public sealed class CashMovementRecord
{
    public Guid Id { get; set; }
    public Guid CashAccountId { get; set; }
    public CashAccountRecord? CashAccount { get; set; }
    public DateOnly Date { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string CurrencyCode { get; set; } = "TRY";
    public string StatusText { get; set; } = "Kayitli";
    public bool IsDeleted { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
