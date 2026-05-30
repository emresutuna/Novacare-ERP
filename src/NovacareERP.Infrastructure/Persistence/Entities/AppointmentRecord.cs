namespace NovacareERP.Infrastructure.Persistence.Entities;

public sealed class AppointmentRecord
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid CustomerId { get; set; }
    public string CustomerName { get; set; } = "";
    public string CompanyName { get; set; } = "";
    public string Title { get; set; } = "";
    public string ContactPerson { get; set; } = "";
    public DateOnly Date { get; set; } = DateOnly.FromDateTime(DateTime.Today);
    public TimeOnly Time { get; set; } = TimeOnly.FromDateTime(DateTime.Now);
    public string CustomerDemand { get; set; } = "";
    public string Notes { get; set; } = "";
    public bool IsDeleted { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}
