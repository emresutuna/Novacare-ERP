using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;
using NovacareERP.Application.Appointments;
using NovacareERP.Application.Customers;
using NovacareERP.Application.Dashboard;
using NovacareERP.Application.PurchaseInvoices;
using NovacareERP.Application.SalesInvoices;
using NovacareERP.Application.Suppliers;
using NovacareERP.Domain.Enums;
using NovacareERP.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Novacare ERP Mobile API",
        Version = "v1",
        Description = "Mobile clients for Novacare ERP."
    });
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("MobileClients", policy =>
    {
        var origins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? [];

        if (origins.Length == 0 || origins.Contains("*"))
        {
            policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
            return;
        }

        policy.WithOrigins(origins).AllowAnyHeader().AllowAnyMethod();
    });
});

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("DefaultConnection connection string is missing.");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddScoped<IDashboardSnapshotService, DashboardSnapshotService>();
builder.Services.AddScoped<ICustomerDirectoryService, EfCustomerDirectoryService>();
builder.Services.AddScoped<IAppointmentDirectoryService, EfAppointmentDirectoryService>();
builder.Services.AddScoped<ISupplierDirectoryService, EfSupplierDirectoryService>();
builder.Services.AddScoped<ISalesInvoiceDirectoryService, EfSalesInvoiceDirectoryService>();
builder.Services.AddScoped<IPurchaseInvoiceDirectoryService, EfPurchaseInvoiceDirectoryService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    DatabaseSeeder.EnsureSeedData(dbContext);
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Novacare ERP Mobile API v1");
        options.RoutePrefix = "swagger";
    });
}

app.UseHttpsRedirection();
app.UseCors("MobileClients");

app.MapGet("/api/health", () => Results.Ok(new
{
    service = "Novacare ERP API",
    status = "Healthy",
    checkedAt = DateTimeOffset.UtcNow
}))
.WithName("Health");

app.MapGet("/api/dashboard/summary", (IDashboardSnapshotService dashboardService) =>
    Results.Ok(dashboardService.GetSummary()))
.WithName("DashboardSummary");

app.MapGet("/api/customers", (
    CustomerLegalType? legalType,
    string? searchTerm,
    ICustomerDirectoryService customerService) =>
    Results.Ok(customerService.GetList(legalType, searchTerm)))
.WithName("GetCustomers");

app.MapGet("/api/customers/{id:guid}", (Guid id, ICustomerDirectoryService customerService) =>
{
    var customer = customerService.GetById(id);
    return customer is null ? Results.NotFound() : Results.Ok(customer);
})
.WithName("GetCustomerById");

app.MapPost("/api/customers", (CustomerFormViewModel form, ICustomerDirectoryService customerService) =>
{
    customerService.Add(form);
    return Results.Created("/api/customers", null);
})
.WithName("CreateCustomer");

app.MapPut("/api/customers/{id:guid}", (Guid id, CustomerFormViewModel form, ICustomerDirectoryService customerService) =>
    customerService.Update(id, form) ? Results.NoContent() : Results.NotFound())
.WithName("UpdateCustomer");

app.MapDelete("/api/customers/{id:guid}", (Guid id, ICustomerDirectoryService customerService) =>
    customerService.Delete(id) ? Results.NoContent() : Results.NotFound())
.WithName("DeleteCustomer");

app.MapGet("/api/suppliers", (
    CustomerLegalType? legalType,
    string? searchTerm,
    ISupplierDirectoryService supplierService) =>
    Results.Ok(supplierService.GetList(legalType, searchTerm)))
.WithName("GetSuppliers");

app.MapGet("/api/suppliers/{id:guid}", (Guid id, ISupplierDirectoryService supplierService) =>
{
    var supplier = supplierService.GetById(id);
    return supplier is null ? Results.NotFound() : Results.Ok(supplier);
})
.WithName("GetSupplierById");

app.MapPost("/api/suppliers", (SupplierFormViewModel form, ISupplierDirectoryService supplierService) =>
{
    supplierService.Add(form);
    return Results.Created("/api/suppliers", null);
})
.WithName("CreateSupplier");

app.MapPut("/api/suppliers/{id:guid}", (Guid id, SupplierFormViewModel form, ISupplierDirectoryService supplierService) =>
    supplierService.Update(id, form) ? Results.NoContent() : Results.NotFound())
.WithName("UpdateSupplier");

app.MapDelete("/api/suppliers/{id:guid}", (Guid id, ISupplierDirectoryService supplierService) =>
    supplierService.Delete(id) ? Results.NoContent() : Results.NotFound())
.WithName("DeleteSupplier");

app.MapGet("/api/appointments", (string? searchTerm, DateOnly? filterDate, IAppointmentDirectoryService appointmentService) =>
    Results.Ok(appointmentService.GetList(searchTerm, filterDate)))
.WithName("GetAppointments");

app.MapGet("/api/appointments/{id:guid}", (Guid id, IAppointmentDirectoryService appointmentService) =>
{
    var appointment = appointmentService.GetById(id);
    return appointment is null ? Results.NotFound() : Results.Ok(appointment);
})
.WithName("GetAppointmentById");

app.MapPost("/api/appointments", (AppointmentFormViewModel form, IAppointmentDirectoryService appointmentService) =>
{
    appointmentService.Add(form);
    return Results.Created("/api/appointments", null);
})
.WithName("CreateAppointment");

app.MapPut("/api/appointments/{id:guid}", (Guid id, AppointmentFormViewModel form, IAppointmentDirectoryService appointmentService) =>
    appointmentService.Update(id, form) ? Results.NoContent() : Results.NotFound())
.WithName("UpdateAppointment");

app.MapDelete("/api/appointments/{id:guid}", (Guid id, IAppointmentDirectoryService appointmentService) =>
    appointmentService.Delete(id) ? Results.NoContent() : Results.NotFound())
.WithName("DeleteAppointment");

app.MapGet("/api/sales-invoices", (
    string? searchTerm,
    InvoiceStatus? status,
    ElectronicDocumentType? documentType,
    ISalesInvoiceDirectoryService invoiceService) =>
    Results.Ok(invoiceService.GetList(searchTerm, status, documentType)))
.WithName("GetSalesInvoices");

app.MapGet("/api/sales-invoices/{id:guid}", (Guid id, ISalesInvoiceDirectoryService invoiceService) =>
{
    var invoice = invoiceService.GetById(id);
    return invoice is null ? Results.NotFound() : Results.Ok(invoice);
})
.WithName("GetSalesInvoiceById");

app.MapGet("/api/sales-invoices/draft", (
    Guid? customerId,
    ElectronicDocumentType? documentType,
    ISalesInvoiceDirectoryService invoiceService) =>
    Results.Ok(invoiceService.GetDraft(customerId, documentType)))
.WithName("GetSalesInvoiceDraft");

app.MapPost("/api/sales-invoices", (SalesInvoiceFormViewModel form, ISalesInvoiceDirectoryService invoiceService) =>
{
    var id = invoiceService.Add(form);
    return Results.Created($"/api/sales-invoices/{id}", new { id });
})
.WithName("CreateSalesInvoice");

app.MapPut("/api/sales-invoices/{id:guid}", (Guid id, SalesInvoiceFormViewModel form, ISalesInvoiceDirectoryService invoiceService) =>
    invoiceService.Update(id, form) ? Results.NoContent() : Results.NotFound())
.WithName("UpdateSalesInvoice");

app.MapDelete("/api/sales-invoices/{id:guid}", (Guid id, ISalesInvoiceDirectoryService invoiceService) =>
    invoiceService.Delete(id) ? Results.NoContent() : Results.NotFound())
.WithName("DeleteSalesInvoice");

app.MapGet("/api/purchase-invoices", (
    string? searchTerm,
    InvoiceStatus? status,
    ElectronicDocumentType? documentType,
    IPurchaseInvoiceDirectoryService invoiceService) =>
    Results.Ok(invoiceService.GetList(searchTerm, status, documentType)))
.WithName("GetPurchaseInvoices");

app.MapGet("/api/purchase-invoices/{id:guid}", (Guid id, IPurchaseInvoiceDirectoryService invoiceService) =>
{
    var invoice = invoiceService.GetById(id);
    return invoice is null ? Results.NotFound() : Results.Ok(invoice);
})
.WithName("GetPurchaseInvoiceById");

app.MapGet("/api/purchase-invoices/draft", (
    Guid? supplierId,
    ElectronicDocumentType? documentType,
    IPurchaseInvoiceDirectoryService invoiceService) =>
    Results.Ok(invoiceService.GetDraft(supplierId, documentType)))
.WithName("GetPurchaseInvoiceDraft");

app.MapPost("/api/purchase-invoices", (PurchaseInvoiceFormViewModel form, IPurchaseInvoiceDirectoryService invoiceService) =>
{
    var id = invoiceService.Add(form);
    return Results.Created($"/api/purchase-invoices/{id}", new { id });
})
.WithName("CreatePurchaseInvoice");

app.MapPut("/api/purchase-invoices/{id:guid}", (Guid id, PurchaseInvoiceFormViewModel form, IPurchaseInvoiceDirectoryService invoiceService) =>
    invoiceService.Update(id, form) ? Results.NoContent() : Results.NotFound())
.WithName("UpdatePurchaseInvoice");

app.MapDelete("/api/purchase-invoices/{id:guid}", (Guid id, IPurchaseInvoiceDirectoryService invoiceService) =>
    invoiceService.Delete(id) ? Results.NoContent() : Results.NotFound())
.WithName("DeletePurchaseInvoice");

app.Run();
