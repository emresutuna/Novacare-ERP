using NovacareERP.Application.Appointments;
using NovacareERP.Application.Customers;
using NovacareERP.Domain.Enums;

namespace NovacareERP.Infrastructure.Persistence;

public sealed class InMemoryAppointmentDirectoryService : IAppointmentDirectoryService
{
    private readonly ICustomerDirectoryService _customerDirectoryService;
    private readonly List<AppointmentListItemViewModel> _appointments = [];

    public InMemoryAppointmentDirectoryService(ICustomerDirectoryService customerDirectoryService)
    {
        _customerDirectoryService = customerDirectoryService;

        // Seed initial mock data from existing customers
        var customers = _customerDirectoryService.GetList(null, null).Customers;
        if (customers.Count > 0)
        {
            var firstCustomer = customers[0];
            _appointments.Add(new AppointmentListItemViewModel(
                Id: Guid.Parse("9a6c6cdd-3ac8-4ad3-8e72-bac1f31dd001"),
                CustomerId: firstCustomer.Id,
                CustomerName: firstCustomer.ContactPerson,
                CompanyName: firstCustomer.DisplayName,
                Title: "İlk Tanışma ve Ürün Sunumu",
                ContactPerson: firstCustomer.ContactPerson,
                Date: DateOnly.FromDateTime(DateTime.Today),
                Time: new TimeOnly(14, 30),
                CustomerDemand: "ERP entegrasyonu ve teklif modülleri hakkında detaylı bilgi istiyor.",
                Notes: "Görüşme olumlu geçti. Teklif hazırlanacak.",
                CreatedAt: DateTime.UtcNow.AddHours(-2)
            ));
        }
    }

    public AppointmentListViewModel GetList(string? searchTerm, DateOnly? filterDate)
    {
        var query = _appointments.AsEnumerable();

        if (filterDate.HasValue)
        {
            query = query.Where(a => a.Date == filterDate.Value);
        }

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(a =>
                a.CustomerName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                a.CompanyName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                a.Title.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                a.ContactPerson.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                a.CustomerDemand.Contains(searchTerm, StringComparison.OrdinalIgnoreCase));
        }

        var customers = _customerDirectoryService.GetList(null, null).Customers;

        return new AppointmentListViewModel
        {
            Appointments = query.OrderByDescending(a => a.Date).ThenByDescending(a => a.Time).ToList(),
            Customers = customers,
            Form = new AppointmentFormViewModel(),
            SearchTerm = searchTerm,
            FilterDate = filterDate
        };
    }

    public AppointmentListItemViewModel? GetById(Guid id)
    {
        return _appointments.FirstOrDefault(a => a.Id == id);
    }

    public void Add(AppointmentFormViewModel form)
    {
        Guid customerId = form.CustomerId;
        string customerName = form.CustomerName;
        string companyName = form.CompanyName;

        if (form.CreateNewCustomer && !string.IsNullOrWhiteSpace(form.NewCustomerDisplayName))
        {
            var customerForm = new CustomerFormViewModel
            {
                DisplayName = form.NewCustomerDisplayName,
                ContactPerson = form.NewCustomerContactPerson,
                Phone = form.NewCustomerPhone,
                Email = form.NewCustomerEmail,
                City = form.NewCustomerCity,
                LegalType = CustomerLegalType.LimitedCompany
            };
            
            _customerDirectoryService.Add(customerForm);

            // Fetch newly added customer to link
            var updatedCustomers = _customerDirectoryService.GetList(null, form.NewCustomerDisplayName).Customers;
            var newCustomer = updatedCustomers.FirstOrDefault(c => c.DisplayName == form.NewCustomerDisplayName);
            if (newCustomer != null)
            {
                customerId = newCustomer.Id;
                customerName = newCustomer.ContactPerson;
                companyName = newCustomer.DisplayName;
            }
            else
            {
                customerName = form.NewCustomerContactPerson;
                companyName = form.NewCustomerDisplayName;
            }
        }
        else
        {
            var customer = _customerDirectoryService.GetById(customerId);
            if (customer != null)
            {
                customerName = customer.ContactPerson;
                companyName = customer.DisplayName;
            }
        }

        _appointments.Add(new AppointmentListItemViewModel(
            Id: Guid.NewGuid(),
            CustomerId: customerId,
            CustomerName: string.IsNullOrWhiteSpace(customerName) ? form.ContactPerson : customerName,
            CompanyName: companyName,
            Title: Clean(form.Title),
            ContactPerson: Clean(form.ContactPerson),
            Date: form.Date,
            Time: form.Time,
            CustomerDemand: Clean(form.CustomerDemand),
            Notes: Clean(form.Notes),
            CreatedAt: DateTime.UtcNow
        ));
    }

    public bool Update(Guid id, AppointmentFormViewModel form)
    {
        var index = _appointments.FindIndex(a => a.Id == id);
        if (index < 0) return false;

        var existing = _appointments[index];
        Guid customerId = form.CustomerId;
        string customerName = form.CustomerName;
        string companyName = form.CompanyName;

        var customer = _customerDirectoryService.GetById(customerId);
        if (customer != null)
        {
            customerName = customer.ContactPerson;
            companyName = customer.DisplayName;
        }

        _appointments[index] = existing with
        {
            CustomerId = customerId,
            CustomerName = string.IsNullOrWhiteSpace(customerName) ? form.ContactPerson : customerName,
            CompanyName = companyName,
            Title = Clean(form.Title),
            ContactPerson = Clean(form.ContactPerson),
            Date = form.Date,
            Time = form.Time,
            CustomerDemand = Clean(form.CustomerDemand),
            Notes = Clean(form.Notes)
        };

        return true;
    }

    public bool Delete(Guid id)
    {
        var item = GetById(id);
        if (item is null) return false;
        return _appointments.Remove(item);
    }

    private static string Clean(string? value) => value?.Trim() ?? "";
}
