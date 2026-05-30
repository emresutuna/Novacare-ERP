using NovacareERP.Application.Customers;
using NovacareERP.Application.Dashboard;

namespace NovacareERP.Infrastructure.Persistence;

public sealed class DashboardSnapshotService : IDashboardSnapshotService
{
    public DashboardSummary GetSummary()
    {
        return new DashboardSummary(
            MonthlyIncome: 184250,
            MonthlyExpense: 67240,
            OpenInvoiceTotal: 43800,
            CashBalance: 129560,
            OpenInvoiceCount: 12,
            CustomerCount: 86);
    }

    public CustomerFormViewModel GetCustomerForm()
    {
        return new CustomerFormViewModel();
    }
}
