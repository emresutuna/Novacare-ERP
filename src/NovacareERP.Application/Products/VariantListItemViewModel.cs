namespace NovacareERP.Application.Products;

public sealed class VariantListItemViewModel
{
    public Guid Id { get; init; }
    public string Name { get; init; } = "";
    public string ValuesText { get; init; } = "";
    public int ProductCount { get; init; }
}
