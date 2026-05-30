using NovacareERP.Domain.Enums;

namespace NovacareERP.Application.Customers;

public interface ICustomerDirectoryService
{
    CustomerListViewModel GetList(CustomerLegalType? legalType, string? searchTerm);
    CustomerListItemViewModel? GetById(Guid id);
    void Add(CustomerFormViewModel form);
    bool Update(Guid id, CustomerFormViewModel form);
    bool Delete(Guid id);
}
