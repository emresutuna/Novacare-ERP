using Microsoft.EntityFrameworkCore;
using NovacareERP.Application.Appointments;
using NovacareERP.Application.Customers;
using NovacareERP.Domain.Enums;
using NovacareERP.Infrastructure.Persistence.Entities;

namespace NovacareERP.Infrastructure.Persistence;

public sealed class EfAppointmentDirectoryService : IAppointmentDirectoryService
{
    private readonly AppDbContext _dbContext;
    private readonly ICustomerDirectoryService _customerDirectoryService;

    public EfAppointmentDirectoryService(
        AppDbContext dbContext,
        ICustomerDirectoryService customerDirectoryService)
    {
        _dbContext = dbContext;
        _customerDirectoryService = customerDirectoryService;
    }

    public AppointmentListViewModel GetList(string? searchTerm, DateOnly? filterDate)
    {
        var query = _dbContext.Appointments
            .AsNoTracking()
            .Where(appointment => !appointment.IsDeleted);

        if (filterDate.HasValue)
        {
            query = query.Where(appointment => appointment.Date == filterDate.Value);
        }

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.Trim();
            query = query.Where(appointment =>
                appointment.CustomerName.Contains(term) ||
                appointment.CompanyName.Contains(term) ||
                appointment.Title.Contains(term) ||
                appointment.ContactPerson.Contains(term) ||
                appointment.CustomerDemand.Contains(term));
        }

        return new AppointmentListViewModel
        {
            Appointments = query
                .OrderByDescending(appointment => appointment.Date)
                .ThenByDescending(appointment => appointment.Time)
                .Select(appointment => ToListItem(appointment))
                .ToList(),
            Customers = _customerDirectoryService.GetList(null, null).Customers,
            Form = new AppointmentFormViewModel(),
            SearchTerm = searchTerm,
            FilterDate = filterDate
        };
    }

    public AppointmentListItemViewModel? GetById(Guid id)
    {
        var appointment = _dbContext.Appointments
            .AsNoTracking()
            .FirstOrDefault(appointment => appointment.Id == id && !appointment.IsDeleted);

        return appointment is null ? null : ToListItem(appointment);
    }

    public void Add(AppointmentFormViewModel form)
    {
        var customerSnapshot = ResolveCustomerSnapshot(form);

        _dbContext.Appointments.Add(new AppointmentRecord
        {
            Id = Guid.NewGuid(),
            CustomerId = customerSnapshot.CustomerId,
            CustomerName = customerSnapshot.CustomerName,
            CompanyName = customerSnapshot.CompanyName,
            Title = string.IsNullOrWhiteSpace(form.Title) ? "Yeni Randevu" : Clean(form.Title),
            ContactPerson = string.IsNullOrWhiteSpace(form.ContactPerson) ? "Emre Sutuna" : Clean(form.ContactPerson),
            Date = form.Date,
            Time = form.Time,
            CustomerDemand = string.IsNullOrWhiteSpace(form.CustomerDemand)
                ? "Musteri ile genel durum degerlendirmesi, ihtiyac analizi ve sonraki adimlar gorusulecek."
                : Clean(form.CustomerDemand),
            Notes = string.IsNullOrWhiteSpace(form.Notes)
                ? "Randevu sonrasi aksiyonlar ve takip notlari bu alanda guncellenecek."
                : Clean(form.Notes),
            CreatedAt = DateTime.UtcNow
        });

        _dbContext.SaveChanges();
    }

    public bool Update(Guid id, AppointmentFormViewModel form)
    {
        var appointment = _dbContext.Appointments
            .FirstOrDefault(appointment => appointment.Id == id && !appointment.IsDeleted);

        if (appointment is null)
        {
            return false;
        }

        var customerSnapshot = ResolveCustomerSnapshot(form);

        appointment.CustomerId = customerSnapshot.CustomerId;
        appointment.CustomerName = customerSnapshot.CustomerName;
        appointment.CompanyName = customerSnapshot.CompanyName;
        appointment.Title = string.IsNullOrWhiteSpace(form.Title) ? appointment.Title : Clean(form.Title);
        appointment.ContactPerson = string.IsNullOrWhiteSpace(form.ContactPerson) ? appointment.ContactPerson : Clean(form.ContactPerson);
        appointment.Date = form.Date;
        appointment.Time = form.Time;
        appointment.CustomerDemand = string.IsNullOrWhiteSpace(form.CustomerDemand) ? appointment.CustomerDemand : Clean(form.CustomerDemand);
        appointment.Notes = string.IsNullOrWhiteSpace(form.Notes) ? appointment.Notes : Clean(form.Notes);
        appointment.UpdatedAt = DateTime.UtcNow;

        _dbContext.SaveChanges();
        return true;
    }

    public bool Delete(Guid id)
    {
        var appointment = _dbContext.Appointments
            .FirstOrDefault(appointment => appointment.Id == id && !appointment.IsDeleted);

        if (appointment is null)
        {
            return false;
        }

        appointment.IsDeleted = true;
        appointment.UpdatedAt = DateTime.UtcNow;
        _dbContext.SaveChanges();
        return true;
    }

    private CustomerSnapshot ResolveCustomerSnapshot(AppointmentFormViewModel form)
    {
        if (form.CreateNewCustomer && !string.IsNullOrWhiteSpace(form.NewCustomerDisplayName))
        {
            var customerForm = new CustomerFormViewModel
            {
                DisplayName = Clean(form.NewCustomerDisplayName),
                ContactPerson = Clean(form.NewCustomerContactPerson),
                Phone = Clean(form.NewCustomerPhone),
                Email = Clean(form.NewCustomerEmail),
                City = Clean(form.NewCustomerCity),
                LegalType = CustomerLegalType.LimitedCompany
            };

            _customerDirectoryService.Add(customerForm);

            var newCustomer = _customerDirectoryService
                .GetList(null, customerForm.DisplayName)
                .Customers
                .FirstOrDefault(customer => customer.DisplayName == customerForm.DisplayName);

            if (newCustomer is not null)
            {
                return new CustomerSnapshot(
                    newCustomer.Id,
                    string.IsNullOrWhiteSpace(newCustomer.ContactPerson) ? newCustomer.DisplayName : newCustomer.ContactPerson,
                    newCustomer.DisplayName);
            }

            return new CustomerSnapshot(
                Guid.Empty,
                Clean(form.NewCustomerContactPerson),
                Clean(form.NewCustomerDisplayName));
        }

        var customer = _customerDirectoryService.GetById(form.CustomerId);
        if (customer is not null)
        {
            return new CustomerSnapshot(
                customer.Id,
                string.IsNullOrWhiteSpace(customer.ContactPerson) ? customer.DisplayName : customer.ContactPerson,
                customer.DisplayName);
        }

        return new CustomerSnapshot(
            form.CustomerId,
            string.IsNullOrWhiteSpace(form.CustomerName) ? "Belirtilmedi" : Clean(form.CustomerName),
            string.IsNullOrWhiteSpace(form.CompanyName) ? "Secili musteri bulunamadi" : Clean(form.CompanyName));
    }

    private static AppointmentListItemViewModel ToListItem(AppointmentRecord appointment)
    {
        return new AppointmentListItemViewModel(
            Id: appointment.Id,
            CustomerId: appointment.CustomerId,
            CustomerName: appointment.CustomerName,
            CompanyName: appointment.CompanyName,
            Title: appointment.Title,
            ContactPerson: appointment.ContactPerson,
            Date: appointment.Date,
            Time: appointment.Time,
            CustomerDemand: appointment.CustomerDemand,
            Notes: appointment.Notes,
            CreatedAt: appointment.CreatedAt);
    }

    private static string Clean(string? value)
    {
        return value?.Trim() ?? "";
    }

    private sealed record CustomerSnapshot(Guid CustomerId, string CustomerName, string CompanyName);
}
