using NovacareERP.Domain.Enums;

namespace NovacareERP.Application.Integrations;

public sealed record ElectronicDocumentRequest(
    Guid InvoiceId,
    InvoiceType InvoiceType,
    ElectronicDocumentType DocumentType,
    string InvoiceNumber,
    string CounterpartyName,
    string TaxNumber,
    decimal GrossAmount,
    string CurrencyCode);

public sealed record ElectronicDocumentResult(
    bool Succeeded,
    string? DocumentUuid,
    string? Ettn,
    string Message);

public sealed record ElectronicDocumentStatusResult(
    string DocumentUuid,
    string? Ettn,
    ElectronicDocumentStatus Status,
    string Message);

public interface IElectronicDocumentIntegration
{
    Task<ElectronicDocumentResult> SendAsync(ElectronicDocumentRequest request, CancellationToken cancellationToken = default);

    Task<ElectronicDocumentStatusResult> GetStatusAsync(string documentUuid, CancellationToken cancellationToken = default);

    Task<ElectronicDocumentResult> CancelAsync(string documentUuid, CancellationToken cancellationToken = default);
}
