namespace NovacareERP.Infrastructure.Persistence.Entities;

public sealed class EmployeeCashSummaryRecord
{
    public Guid Id { get; set; }
    public string EmployeeName { get; set; } = string.Empty;
    public string RoleName { get; set; } = string.Empty;
    public decimal AdvanceBalance { get; set; }
    public decimal ExpenseTotal { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
