using NovacareERP.Application.Customers;

namespace NovacareERP.Application.Dashboard;

public interface IDashboardSnapshotService
{
    DashboardSummary GetSummary();
    CustomerFormViewModel GetCustomerForm();
}
