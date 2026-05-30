namespace NovacareERP.Application.Products;

public sealed class ProductDirectoryViewModel
{
    public string? SearchTerm { get; init; }
    public string? CategoryName { get; init; }
    public string? BrandName { get; init; }
    public IReadOnlyList<ProductListItemViewModel> Products { get; init; } = [];
    public IReadOnlyList<string> Categories { get; init; } = [];
    public IReadOnlyList<string> Brands { get; init; } = [];
    public int ActiveProductCount => Products.Count;
    public int TotalStockQuantity => Products.Sum(product => product.StockQuantity);
}
