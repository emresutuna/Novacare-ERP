using NovacareERP.Application.Products;

namespace NovacareERP.Infrastructure.Persistence;

public sealed class InMemoryProductDirectoryService : IProductDirectoryService
{
    private readonly List<ProductListItemViewModel> _products =
    [
        new()
        {
            Id = Guid.Parse("7f2fe5e4-f2e7-43a1-8c15-418bf31a0f57"),
            Name = "Microsoft Mouse",
            CategoryName = "Cevre Birimleri",
            BrandName = "Microsoft",
            SalesPrice = 2500m,
            StockQuantity = 0,
            UnitName = "ad",
            LocationName = "Mouse Yeri"
        }
    ];

    private readonly List<WarehouseListItemViewModel> _warehouses =
    [
        new()
        {
            Id = Guid.Parse("1a277d83-cb69-41a3-b089-7ee5e8d4eb43"),
            Name = "Ana Depo",
            Code = "DP-001",
            ProductCount = 1,
            ResponsibleName = "Emre Sutuna"
        }
    ];

    private readonly List<PriceListItemViewModel> _priceLists =
    [
        new()
        {
            Id = Guid.Parse("8fe15670-4d95-46ff-a848-975e93a4f415"),
            Name = "Standart Satis Fiyatlari",
            CurrencyCode = "TL",
            ProductCount = 1,
            SegmentName = "Tum Musteriler"
        }
    ];

    private readonly List<CatalogListItemViewModel> _catalogs =
    [
        new()
        {
            Id = Guid.Parse("c4c99f72-d487-4826-98e2-8236fdc9bc61"),
            Name = "2026 Urun Katalogu",
            VisibilityText = "Fiyat ve stok gizli",
            ProductCount = 1,
            UpdatedAt = new DateOnly(2026, 5, 30)
        }
    ];

    private readonly List<VariantListItemViewModel> _variants =
    [
        new()
        {
            Id = Guid.Parse("1b64eacf-4133-4d2b-9f5e-1f76036e4651"),
            Name = "Renk",
            ValuesText = "Siyah, Beyaz, Gri",
            ProductCount = 1
        },
        new()
        {
            Id = Guid.Parse("f8186d48-c7fa-43be-a85e-266786142ba3"),
            Name = "Ebat",
            ValuesText = "Kucuk, Orta, Buyuk",
            ProductCount = 0
        }
    ];

    public ProductDirectoryViewModel GetProducts(string? searchTerm, string? categoryName, string? brandName)
    {
        var query = _products.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(product => product.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(categoryName))
        {
            query = query.Where(product => product.CategoryName.Equals(categoryName, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(brandName))
        {
            query = query.Where(product => product.BrandName.Equals(brandName, StringComparison.OrdinalIgnoreCase));
        }

        return new ProductDirectoryViewModel
        {
            SearchTerm = searchTerm,
            CategoryName = categoryName,
            BrandName = brandName,
            Products = query.ToList(),
            Categories = _products.Select(product => product.CategoryName).Distinct().ToList(),
            Brands = _products.Select(product => product.BrandName).Distinct().ToList()
        };
    }

    public ProductWorkspaceViewModel GetWorkspace()
    {
        return new ProductWorkspaceViewModel
        {
            Directory = GetProducts(null, null, null),
            Warehouses = _warehouses,
            PriceLists = _priceLists,
            Catalogs = _catalogs,
            Variants = _variants
        };
    }
}
