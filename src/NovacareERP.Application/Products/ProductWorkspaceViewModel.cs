namespace NovacareERP.Application.Products;

public sealed class ProductWorkspaceViewModel
{
    public ProductDirectoryViewModel Directory { get; init; } = new();
    public IReadOnlyList<WarehouseListItemViewModel> Warehouses { get; init; } = [];
    public IReadOnlyList<PriceListItemViewModel> PriceLists { get; init; } = [];
    public IReadOnlyList<CatalogListItemViewModel> Catalogs { get; init; } = [];
    public IReadOnlyList<VariantListItemViewModel> Variants { get; init; } = [];
}
