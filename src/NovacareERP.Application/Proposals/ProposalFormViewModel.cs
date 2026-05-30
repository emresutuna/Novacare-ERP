namespace NovacareERP.Application.Proposals;

public sealed class ProposalFormViewModel
{
    public Guid Id { get; init; }
    public string CustomerName { get; init; } = "Eyup Biradli";
    public string ProposalNumber { get; init; } = "1";
    public DateOnly ProposalDate { get; init; } = new(2026, 5, 30);
    public DateOnly ValidUntil { get; init; } = new(2026, 5, 30);
    public string CurrencyCode { get; init; } = "TRY";
    public decimal ExchangeRate { get; init; } = 1;
    public string Description { get; init; } = "";
    public IReadOnlyList<ProposalLineItemViewModel> Items { get; init; } =
    [
        new("Microsoft Mouse", 1, 1250000, 20, 1250000)
    ];
}
