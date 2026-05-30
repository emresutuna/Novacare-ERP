namespace NovacareERP.Application.Proposals;

public sealed class ProposalListViewModel
{
    public IReadOnlyList<ProposalListItemViewModel> Proposals { get; init; } = [];
    public string PeriodText { get; init; } = "Son Bir Yil";
    public string? SearchTerm { get; init; }
    public bool IncludeCancelled { get; init; }
}
