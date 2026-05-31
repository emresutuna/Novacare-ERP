using Microsoft.EntityFrameworkCore;
using NovacareERP.Application.Invoices;
using NovacareERP.Application.PurchaseInvoices;
using NovacareERP.Application.Suppliers;
using NovacareERP.Domain.Enums;
using NovacareERP.Infrastructure.Persistence.Entities;

namespace NovacareERP.Infrastructure.Persistence;

public sealed class EfPurchaseInvoiceDirectoryService : IPurchaseInvoiceDirectoryService
{
    private readonly AppDbContext _dbContext;
    private readonly ISupplierDirectoryService _supplierDirectoryService;

    public EfPurchaseInvoiceDirectoryService(AppDbContext dbContext, ISupplierDirectoryService supplierDirectoryService)
    {
        _dbContext = dbContext;
        _supplierDirectoryService = supplierDirectoryService;
    }

    public PurchaseInvoiceListViewModel GetList(string? searchTerm, InvoiceStatus? status, ElectronicDocumentType? documentType)
    {
        var query = _dbContext.Invoices.AsNoTracking().Where(invoice => invoice.InvoiceType == InvoiceType.Purchase && !invoice.IsDeleted);

        if (status.HasValue) query = query.Where(invoice => invoice.Status == status.Value);
        if (documentType.HasValue) query = query.Where(invoice => invoice.DocumentType == documentType.Value);

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.Trim();
            query = query.Where(invoice => invoice.PartyName.Contains(term) || invoice.InvoiceNumber.Contains(term));
        }

        return new PurchaseInvoiceListViewModel
        {
            Invoices = query
                .Include(invoice => invoice.Items)
                .OrderByDescending(invoice => invoice.IssueDate)
                .Select(invoice => ToListItem(invoice))
                .ToList(),
            SearchTerm = searchTerm,
            Status = status,
            DocumentType = documentType
        };
    }

    public PurchaseInvoiceFormViewModel? GetById(Guid id)
    {
        var invoice = _dbContext.Invoices
            .AsNoTracking()
            .Include(invoice => invoice.Items)
            .FirstOrDefault(invoice => invoice.Id == id && invoice.InvoiceType == InvoiceType.Purchase && !invoice.IsDeleted);

        return invoice is null ? null : ToForm(invoice);
    }

    public PurchaseInvoiceFormViewModel GetDraft(Guid? supplierId = null, ElectronicDocumentType? documentType = null)
    {
        var supplier = supplierId.HasValue ? _supplierDirectoryService.GetById(supplierId.Value) : null;

        return new PurchaseInvoiceFormViewModel
        {
            SupplierId = supplier?.Id,
            SupplierName = supplier?.DisplayName ?? "",
            SupplierTaxNumber = supplier?.TaxNumber ?? "",
            InvoiceNumber = GetNextInvoiceNumber(),
            IssueDate = DateOnly.FromDateTime(DateTime.Today),
            DueDate = DateOnly.FromDateTime(DateTime.Today.AddDays(30)),
            DocumentType = documentType ?? ElectronicDocumentType.EFatura,
            Items = [CreateDefaultLine()]
        };
    }

    public Guid Add(PurchaseInvoiceFormViewModel form)
    {
        var invoice = new InvoiceRecord
        {
            Id = Guid.NewGuid(),
            InvoiceType = InvoiceType.Purchase,
            SupplierId = form.SupplierId,
            PartyName = Clean(form.SupplierName),
            PartyTaxNumber = Clean(form.SupplierTaxNumber),
            InvoiceNumber = string.IsNullOrWhiteSpace(form.InvoiceNumber) ? GetNextInvoiceNumber() : Clean(form.InvoiceNumber),
            IssueDate = form.IssueDate,
            DueDate = form.DueDate,
            CurrencyCode = NormalizeCurrencyCode(form.CurrencyCode),
            ExchangeRate = form.ExchangeRate <= 0 ? 1 : form.ExchangeRate,
            Description = Clean(form.Description),
            Status = form.Status,
            DocumentType = form.DocumentType,
            DocumentStatus = form.DocumentStatus,
            DocumentUuid = CleanNullable(form.DocumentUuid),
            Ettn = CleanNullable(form.Ettn),
            CreatedAt = DateTime.UtcNow,
            Items = ToLineRecords(form.Items)
        };

        _dbContext.Invoices.Add(invoice);
        _dbContext.SaveChanges();
        return invoice.Id;
    }

    public bool Update(Guid id, PurchaseInvoiceFormViewModel form)
    {
        var invoice = _dbContext.Invoices
            .Include(invoice => invoice.Items)
            .FirstOrDefault(invoice => invoice.Id == id && invoice.InvoiceType == InvoiceType.Purchase && !invoice.IsDeleted);

        if (invoice is null) return false;

        invoice.SupplierId = form.SupplierId;
        invoice.PartyName = Clean(form.SupplierName);
        invoice.PartyTaxNumber = Clean(form.SupplierTaxNumber);
        invoice.InvoiceNumber = Clean(form.InvoiceNumber);
        invoice.IssueDate = form.IssueDate;
        invoice.DueDate = form.DueDate;
        invoice.CurrencyCode = NormalizeCurrencyCode(form.CurrencyCode);
        invoice.ExchangeRate = form.ExchangeRate <= 0 ? 1 : form.ExchangeRate;
        invoice.Description = Clean(form.Description);
        invoice.Status = form.Status;
        invoice.DocumentType = form.DocumentType;
        invoice.DocumentStatus = form.DocumentStatus;
        invoice.DocumentUuid = CleanNullable(form.DocumentUuid);
        invoice.Ettn = CleanNullable(form.Ettn);
        invoice.UpdatedAt = DateTime.UtcNow;
        ReplaceLines(invoice, form.Items);

        _dbContext.SaveChanges();
        return true;
    }

    public bool Delete(Guid id)
    {
        var invoice = _dbContext.Invoices.FirstOrDefault(invoice => invoice.Id == id && invoice.InvoiceType == InvoiceType.Purchase && !invoice.IsDeleted);
        if (invoice is null) return false;

        invoice.IsDeleted = true;
        invoice.UpdatedAt = DateTime.UtcNow;
        _dbContext.SaveChanges();
        return true;
    }

    public bool ApplyElectronicDocumentResult(Guid id, string documentUuid, string ettn, ElectronicDocumentStatus status)
    {
        var invoice = _dbContext.Invoices.FirstOrDefault(invoice => invoice.Id == id && invoice.InvoiceType == InvoiceType.Purchase && !invoice.IsDeleted);
        if (invoice is null) return false;

        invoice.DocumentUuid = Clean(documentUuid);
        invoice.Ettn = Clean(ettn);
        invoice.DocumentStatus = status;
        invoice.Status = status == ElectronicDocumentStatus.Accepted ? InvoiceStatus.Issued : invoice.Status;
        invoice.UpdatedAt = DateTime.UtcNow;
        _dbContext.SaveChanges();
        return true;
    }

    private string GetNextInvoiceNumber()
    {
        var year = DateTime.Today.Year;
        var count = _dbContext.Invoices.Count(invoice => invoice.InvoiceType == InvoiceType.Purchase) + 1;
        string number;

        do
        {
            number = $"AF-{year}-{count:0000}";
            count++;
        }
        while (_dbContext.Invoices.Any(invoice => invoice.InvoiceNumber == number));

        return number;
    }

    private static PurchaseInvoiceListItemViewModel ToListItem(InvoiceRecord invoice) =>
        new(
            invoice.Id,
            invoice.IssueDate,
            invoice.PartyName,
            invoice.InvoiceNumber,
            CalculateGross(invoice.Items),
            invoice.CurrencyCode,
            invoice.Status,
            InvoiceDisplayText.GetStatusText(invoice.Status),
            invoice.DocumentType,
            InvoiceDisplayText.GetDocumentTypeText(invoice.DocumentType),
            invoice.DocumentStatus,
            InvoiceDisplayText.GetDocumentStatusText(invoice.DocumentStatus));

    private static PurchaseInvoiceFormViewModel ToForm(InvoiceRecord invoice) =>
        new()
        {
            Id = invoice.Id,
            Title = "Alis Faturasi Duzenle",
            SupplierId = invoice.SupplierId,
            SupplierName = invoice.PartyName,
            SupplierTaxNumber = invoice.PartyTaxNumber,
            InvoiceNumber = invoice.InvoiceNumber,
            IssueDate = invoice.IssueDate,
            DueDate = invoice.DueDate,
            CurrencyCode = invoice.CurrencyCode,
            ExchangeRate = invoice.ExchangeRate,
            Description = invoice.Description,
            Status = invoice.Status,
            DocumentType = invoice.DocumentType,
            DocumentStatus = invoice.DocumentStatus,
            DocumentUuid = invoice.DocumentUuid,
            Ettn = invoice.Ettn,
            Items = invoice.Items.OrderBy(line => line.SortOrder).Select(ToLineViewModel).ToList()
        };

    private static void ReplaceLines(InvoiceRecord invoice, IReadOnlyList<InvoiceLineItemViewModel> items)
    {
        invoice.Items.Clear();
        invoice.Items.AddRange(ToLineRecords(items));
    }

    private static List<InvoiceLineRecord> ToLineRecords(IReadOnlyList<InvoiceLineItemViewModel> items)
    {
        var source = items.Count == 0 ? [CreateDefaultLine()] : items;
        return source.Select((item, index) => new InvoiceLineRecord
        {
            ProductOrServiceName = Clean(item.ProductOrServiceName),
            Quantity = item.Quantity,
            UnitPrice = item.UnitPrice,
            TaxRate = item.TaxRate,
            TotalAmount = item.TotalAmount,
            SortOrder = index
        }).ToList();
    }

    private static InvoiceLineItemViewModel ToLineViewModel(InvoiceLineRecord line) =>
        new()
        {
            ProductOrServiceName = line.ProductOrServiceName,
            Quantity = line.Quantity,
            UnitPrice = line.UnitPrice,
            TaxRate = line.TaxRate,
            TotalAmount = line.TotalAmount
        };

    private static InvoiceLineItemViewModel CreateDefaultLine() =>
        new() { ProductOrServiceName = "Urun / Hizmet", Quantity = 1, UnitPrice = 0, TaxRate = 20, TotalAmount = 0 };

    private static decimal CalculateGross(IReadOnlyList<InvoiceLineRecord> items) =>
        items.Sum(item => item.TotalAmount + item.TotalAmount * item.TaxRate / 100m);

    private static string Clean(string? value) => value?.Trim() ?? "";
    private static string? CleanNullable(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    private static string NormalizeCurrencyCode(string? value)
    {
        var currencyCode = Clean(value).ToUpperInvariant();
        return currencyCode is "TRY" or "USD" or "EUR" ? currencyCode : "TRY";
    }
}
