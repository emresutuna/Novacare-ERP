using NovacareERP.Domain.Enums;

namespace NovacareERP.Application.Suppliers;

public interface ISupplierDirectoryService
{
    SupplierListViewModel GetList(CustomerLegalType? legalType, string? searchTerm);
    SupplierListItemViewModel? GetById(Guid id);
    void Add(SupplierFormViewModel form);
    bool Update(Guid id, SupplierFormViewModel form);
    bool Delete(Guid id);
}
