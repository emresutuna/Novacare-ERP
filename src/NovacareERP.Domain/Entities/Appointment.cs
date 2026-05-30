using NovacareERP.Domain.Common;

namespace NovacareERP.Domain.Entities;

public sealed class Appointment : BaseEntity
{
    public Appointment(
        Guid customerId,
        string customerName,
        string companyName,
        string title,
        string contactPerson,
        DateOnly date,
        TimeOnly time,
        string customerDemand,
        string notes)
    {
        CustomerId = customerId;
        CustomerName = customerName;
        CompanyName = companyName;
        Title = title;
        ContactPerson = contactPerson;
        Date = date;
        Time = time;
        CustomerDemand = customerDemand;
        Notes = notes;
    }

    public Guid CustomerId { get; private set; }
    public string CustomerName { get; private set; }
    public string CompanyName { get; private set; }
    public string Title { get; private set; }
    public string ContactPerson { get; private set; }
    public DateOnly Date { get; private set; }
    public TimeOnly Time { get; private set; }
    public string CustomerDemand { get; private set; }
    public string Notes { get; private set; }

    public void Update(
        Guid customerId,
        string customerName,
        string companyName,
        string title,
        string contactPerson,
        DateOnly date,
        TimeOnly time,
        string customerDemand,
        string notes)
    {
        CustomerId = customerId;
        CustomerName = customerName;
        CompanyName = companyName;
        Title = title;
        ContactPerson = contactPerson;
        Date = date;
        Time = time;
        CustomerDemand = customerDemand;
        Notes = notes;
        MarkUpdated();
    }
}
