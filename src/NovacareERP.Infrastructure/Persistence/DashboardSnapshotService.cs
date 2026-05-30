using NovacareERP.Application.Customers;
using NovacareERP.Application.Dashboard;

namespace NovacareERP.Infrastructure.Persistence;

public sealed class DashboardSnapshotService : IDashboardSnapshotService
{
    private readonly AppDbContext _dbContext;

    public DashboardSnapshotService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public DashboardSummary GetSummary()
    {
        var today = DateOnly.FromDateTime(DateTime.Today);
        var monthStart = today.AddMonths(-1);

        var recentAppointments = _dbContext.Appointments
            .Where(appointment =>
                !appointment.IsDeleted &&
                appointment.Date >= monthStart &&
                appointment.Date <= today)
            .OrderByDescending(appointment => appointment.Date)
            .ThenByDescending(appointment => appointment.Time)
            .Take(6)
            .Select(appointment => new DashboardAppointmentSummary(
                appointment.Id,
                appointment.Title,
                appointment.CompanyName,
                appointment.CustomerName,
                appointment.ContactPerson,
                appointment.Date,
                appointment.Time,
                appointment.CustomerDemand))
            .ToList();

        return new DashboardSummary(
            MonthlyIncome: 184250,
            MonthlyExpense: 67240,
            OpenInvoiceTotal: 43800,
            CashBalance: 129560,
            OpenInvoiceCount: 12,
            CustomerCount: _dbContext.Customers.Count(customer => !customer.IsDeleted),
            RecentAppointments: recentAppointments);
    }

    public CustomerFormViewModel GetCustomerForm()
    {
        return new CustomerFormViewModel();
    }
}
