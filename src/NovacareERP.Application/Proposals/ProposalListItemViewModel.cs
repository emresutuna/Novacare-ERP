namespace NovacareERP.Application.Proposals;

public sealed record ProposalListItemViewModel(
    Guid Id,
    DateOnly ProposalDate,
    DateOnly ValidUntil,
    string CustomerName,
    string ProposalNumber,
    decimal TotalAmount,
    string CurrencyCode,
    bool IsCancelled);
