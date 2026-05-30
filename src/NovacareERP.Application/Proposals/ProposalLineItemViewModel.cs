namespace NovacareERP.Application.Proposals;

public sealed record ProposalLineItemViewModel(
    string ProductOrServiceName,
    decimal Quantity,
    decimal UnitPrice,
    decimal TaxRate,
    decimal TotalAmount);
