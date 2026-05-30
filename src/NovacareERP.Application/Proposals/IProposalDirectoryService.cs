namespace NovacareERP.Application.Proposals;

public interface IProposalDirectoryService
{
    ProposalListViewModel GetList(string? searchTerm, bool includeCancelled);
    ProposalFormViewModel GetDraft(Guid? customerId = null);
}
