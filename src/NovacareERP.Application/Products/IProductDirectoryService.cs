namespace NovacareERP.Application.Products;

public interface IProductDirectoryService
{
    ProductDirectoryViewModel GetProducts(string? searchTerm, string? categoryName, string? brandName);
    ProductWorkspaceViewModel GetWorkspace();
}
