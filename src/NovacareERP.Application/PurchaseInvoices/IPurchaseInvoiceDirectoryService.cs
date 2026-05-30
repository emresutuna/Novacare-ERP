using NovacareERP.Domain.Enums;

namespace NovacareERP.Application.PurchaseInvoices;

public interface IPurchaseInvoiceDirectoryService
{
    PurchaseInvoiceListViewModel GetList(string? searchTerm, InvoiceStatus? status, ElectronicDocumentType? documentType);

    PurchaseInvoiceFormViewModel? GetById(Guid id);

    PurchaseInvoiceFormViewModel GetDraft(Guid? supplierId = null, ElectronicDocumentType? documentType = null);

    Guid Add(PurchaseInvoiceFormViewModel form);

    bool Update(Guid id, PurchaseInvoiceFormViewModel form);

    bool Delete(Guid id);

    bool ApplyElectronicDocumentResult(Guid id, string documentUuid, string ettn, ElectronicDocumentStatus status);
}
