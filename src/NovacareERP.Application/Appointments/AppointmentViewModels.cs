using NovacareERP.Application.Customers;

namespace NovacareERP.Application.Appointments;

public record AppointmentListItemViewModel(
    Guid Id,
    Guid CustomerId,
    string CustomerName,
    string CompanyName,
    string Title,
    string ContactPerson,
    DateOnly Date,
    TimeOnly Time,
    string CustomerDemand,
    string Notes,
    DateTime CreatedAt
);

public sealed class AppointmentFormViewModel
{
    public Guid Id { get; init; }
    public Guid CustomerId { get; init; }

    // On-the-fly Customer Creation Toggle and Fields
    public bool CreateNewCustomer { get; init; }
    public string NewCustomerDisplayName { get; init; } = "";
    public string NewCustomerContactPerson { get; init; } = "";
    public string NewCustomerPhone { get; init; } = "";
    public string NewCustomerEmail { get; init; } = "";
    public string NewCustomerCity { get; init; } = "";

    // Appointment Core Fields
    public string CustomerName { get; init; } = "";
    public string CompanyName { get; init; } = "";
    public string Title { get; init; } = "";
    public string ContactPerson { get; init; } = "";
    public DateOnly Date { get; init; } = DateOnly.FromDateTime(DateTime.Today);
    public TimeOnly Time { get; init; } = TimeOnly.FromDateTime(DateTime.Now);
    public string CustomerDemand { get; init; } = "";
    public string Notes { get; init; } = "";
}

public sealed class AppointmentListViewModel
{
    public IReadOnlyList<AppointmentListItemViewModel> Appointments { get; init; } = [];
    public IReadOnlyList<CustomerListItemViewModel> Customers { get; init; } = [];
    public AppointmentFormViewModel Form { get; init; } = new();
    public string? SearchTerm { get; init; }
    public DateOnly? FilterDate { get; init; }
}
