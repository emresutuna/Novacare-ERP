namespace NovacareERP.Application.Dashboard;

public sealed record DashboardSummary(
    decimal MonthlyIncome,
    decimal MonthlyExpense,
    decimal OpenInvoiceTotal,
    decimal CashBalance,
    int OpenInvoiceCount,
    int CustomerCount);
