using NovacareERP.Domain.Common;
using NovacareERP.Domain.Enums;

namespace NovacareERP.Domain.Entities;

public sealed class CurrentAccount : BaseEntity, ICompanyScoped
{
    public CurrentAccount(Guid companyId, string code, string name, CurrentAccountType type, string currencyCode = "TRY")
    {
        CompanyId = companyId;
        Code = code;
        Name = name;
        Type = type;
        CurrencyCode = currencyCode;
    }

    public Guid CompanyId { get; private set; }
    public string Code { get; private set; }
    public string Name { get; private set; }
    public CurrentAccountType Type { get; private set; }
    public string CurrencyCode { get; private set; }
    public decimal Balance { get; private set; }

    public void ApplyDebit(decimal amount)
    {
        Balance += amount;
        MarkUpdated();
    }

    public void ApplyCredit(decimal amount)
    {
        Balance -= amount;
        MarkUpdated();
    }
}
