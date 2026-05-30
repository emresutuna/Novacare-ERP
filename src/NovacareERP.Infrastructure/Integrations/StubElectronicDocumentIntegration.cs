using NovacareERP.Application.Integrations;
using NovacareERP.Domain.Enums;

namespace NovacareERP.Infrastructure.Integrations;

public sealed class StubElectronicDocumentIntegration : IElectronicDocumentIntegration
{
    private readonly Dictionary<string, ElectronicDocumentStatusResult> _documents = new(StringComparer.OrdinalIgnoreCase);

    public Task<ElectronicDocumentResult> SendAsync(ElectronicDocumentRequest request, CancellationToken cancellationToken = default)
    {
        var documentUuid = Guid.NewGuid().ToString();
        var ettn = Guid.NewGuid().ToString("N")[..16].ToUpperInvariant();
        var message = request.DocumentType switch
        {
            ElectronicDocumentType.EFatura => "E-fatura entegrator kuyruguna alindi.",
            ElectronicDocumentType.EArsiv => "E-arşiv faturasi entegrator kuyruguna alindi.",
            ElectronicDocumentType.EIrsaliye => "E-irsaliye entegrator kuyruguna alindi.",
            _ => "Belge entegrator kuyruguna alindi."
        };

        _documents[documentUuid] = new ElectronicDocumentStatusResult(
            documentUuid,
            ettn,
            ElectronicDocumentStatus.Pending,
            "GIB onayi bekleniyor.");

        return Task.FromResult(new ElectronicDocumentResult(true, documentUuid, ettn, message));
    }

    public Task<ElectronicDocumentStatusResult> GetStatusAsync(string documentUuid, CancellationToken cancellationToken = default)
    {
        if (!_documents.TryGetValue(documentUuid, out var current))
        {
            return Task.FromResult(new ElectronicDocumentStatusResult(
                documentUuid,
                null,
                ElectronicDocumentStatus.NotSent,
                "Belge bulunamadi."));
        }

        if (current.Status == ElectronicDocumentStatus.Pending)
        {
            current = current with
            {
                Status = ElectronicDocumentStatus.Accepted,
                Message = "GIB tarafindan onaylandi."
            };
            _documents[documentUuid] = current;
        }

        return Task.FromResult(current);
    }

    public Task<ElectronicDocumentResult> CancelAsync(string documentUuid, CancellationToken cancellationToken = default)
    {
        if (!_documents.TryGetValue(documentUuid, out var current))
        {
            return Task.FromResult(new ElectronicDocumentResult(false, documentUuid, current?.Ettn, "Iptal edilecek belge bulunamadi."));
        }

        _documents[documentUuid] = current with
        {
            Status = ElectronicDocumentStatus.Cancelled,
            Message = "Belge iptal edildi."
        };

        return Task.FromResult(new ElectronicDocumentResult(true, documentUuid, current.Ettn, "Belge basariyla iptal edildi."));
    }
}
