using NovacareERP.Application.Proposals;

namespace NovacareERP.Infrastructure.Persistence;

public sealed class InMemoryProposalDirectoryService : IProposalDirectoryService
{
    private readonly List<ProposalListItemViewModel> _proposals =
    [
        new(
            Id: Guid.Parse("8d6c6cdd-3ac8-4ad3-8e72-bac1f31dd001"),
            ProposalDate: new DateOnly(2026, 5, 30),
            ValidUntil: new DateOnly(2026, 5, 30),
            CustomerName: "Eyup Biradli",
            ProposalNumber: "1",
            TotalAmount: 1250000,
            CurrencyCode: "TRY",
            IsCancelled: false)
    ];

    public ProposalListViewModel GetList(string? searchTerm, bool includeCancelled)
    {
        var query = _proposals.AsEnumerable();

        if (!includeCancelled)
        {
            query = query.Where(proposal => !proposal.IsCancelled);
        }

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(proposal =>
                proposal.CustomerName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                proposal.ProposalNumber.Contains(searchTerm, StringComparison.OrdinalIgnoreCase));
        }

        return new ProposalListViewModel
        {
            Proposals = query.OrderByDescending(proposal => proposal.ProposalDate).ToList(),
            SearchTerm = searchTerm,
            IncludeCancelled = includeCancelled
        };
    }

    public ProposalFormViewModel GetDraft(Guid? customerId = null)
    {
        return new ProposalFormViewModel();
    }
}
