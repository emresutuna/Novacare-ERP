namespace NovacareERP.Domain.Common;

public interface ICompanyScoped
{
    Guid CompanyId { get; }
}
