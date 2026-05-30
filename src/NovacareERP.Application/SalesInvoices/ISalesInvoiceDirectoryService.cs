using NovacareERP.Domain.Enums;

namespace NovacareERP.Application.SalesInvoices;

public interface ISalesInvoiceDirectoryService
{
    SalesInvoiceListViewModel GetList(string? searchTerm, InvoiceStatus? status, ElectronicDocumentType? documentType);

    SalesInvoiceFormViewModel? GetById(Guid id);

    SalesInvoiceFormViewModel GetDraft(Guid? customerId = null, ElectronicDocumentType? documentType = null);

    Guid Add(SalesInvoiceFormViewModel form);

    bool Update(Guid id, SalesInvoiceFormViewModel form);

    bool Delete(Guid id);

    bool ApplyElectronicDocumentResult(Guid id, string documentUuid, string ettn, ElectronicDocumentStatus status);
}
