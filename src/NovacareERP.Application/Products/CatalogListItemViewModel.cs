namespace NovacareERP.Application.Products;

public sealed class CatalogListItemViewModel
{
    public Guid Id { get; init; }
    public string Name { get; init; } = "";
    public string VisibilityText { get; init; } = "";
    public int ProductCount { get; init; }
    public DateOnly UpdatedAt { get; init; }
}
