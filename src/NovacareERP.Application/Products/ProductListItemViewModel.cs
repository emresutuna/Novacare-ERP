namespace NovacareERP.Application.Products;

public sealed class ProductListItemViewModel
{
    public Guid Id { get; init; }
    public string Name { get; init; } = "";
    public string CategoryName { get; init; } = "";
    public string BrandName { get; init; } = "";
    public decimal SalesPrice { get; init; }
    public int StockQuantity { get; init; }
    public string UnitName { get; init; } = "ad";
    public string LocationName { get; init; } = "";
}
