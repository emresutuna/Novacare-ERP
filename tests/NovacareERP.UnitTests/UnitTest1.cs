using NovacareERP.Domain.Entities;

namespace NovacareERP.UnitTests;

public class InvoiceCalculationTests
{
    [Fact]
    public void AddItem_Calculates_Net_Tax_And_Gross_Amounts()
    {
        var invoice = new Invoice(Guid.NewGuid(), Guid.NewGuid(), "NVC-000001", new DateOnly(2026, 5, 30));

        invoice.AddItem("Danismanlik hizmeti", 2, 1000, 20);

        Assert.Equal(2000, invoice.NetAmount);
        Assert.Equal(400, invoice.TaxAmount);
        Assert.Equal(2400, invoice.GrossAmount);
    }
}
