namespace NovacareERP.Application.Products;

public sealed class WarehouseListItemViewModel
{
    public Guid Id { get; init; }
    public string Name { get; init; } = "";
    public string Code { get; init; } = "";
    public int ProductCount { get; init; }
    public string ResponsibleName { get; init; } = "";
}
