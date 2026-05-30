namespace NovacareERP.Application.Products;

public sealed class PriceListItemViewModel
{
    public Guid Id { get; init; }
    public string Name { get; init; } = "";
    public string CurrencyCode { get; init; } = "TL";
    public int ProductCount { get; init; }
    public string SegmentName { get; init; } = "";
}
