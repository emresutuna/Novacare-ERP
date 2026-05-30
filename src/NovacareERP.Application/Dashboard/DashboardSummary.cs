namespace NovacareERP.Application.Dashboard;

public sealed record DashboardAppointmentSummary(
    Guid Id,
    string Title,
    string CompanyName,
    string CustomerName,
    string ContactPerson,
    DateOnly Date,
    TimeOnly Time,
    string CustomerDemand);

public sealed record DashboardSummary(
    decimal MonthlyIncome,
    decimal MonthlyExpense,
    decimal OpenInvoiceTotal,
    decimal CashBalance,
    int OpenInvoiceCount,
    int CustomerCount,
    IReadOnlyList<DashboardAppointmentSummary> RecentAppointments);
