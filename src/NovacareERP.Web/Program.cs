using Microsoft.EntityFrameworkCore;
using NovacareERP.Application.Dashboard;
using NovacareERP.Application.Customers;
using NovacareERP.Application.Appointments;
using NovacareERP.Application.Integrations;
using NovacareERP.Application.Proposals;
using NovacareERP.Application.PurchaseInvoices;
using NovacareERP.Application.Products;
using NovacareERP.Application.SalesInvoices;
using NovacareERP.Application.Settings;
using NovacareERP.Application.Suppliers;
using NovacareERP.Infrastructure.Integrations;
using NovacareERP.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("DefaultConnection connection string is missing.");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddScoped<IDashboardSnapshotService, DashboardSnapshotService>();
builder.Services.AddScoped<ICustomerDirectoryService, EfCustomerDirectoryService>();
builder.Services.AddScoped<IAppointmentDirectoryService, EfAppointmentDirectoryService>();
builder.Services.AddSingleton<ISupplierDirectoryService, InMemorySupplierDirectoryService>();
builder.Services.AddSingleton<IProposalDirectoryService, InMemoryProposalDirectoryService>();
builder.Services.AddSingleton<IProductDirectoryService, InMemoryProductDirectoryService>();
builder.Services.AddScoped<ISalesInvoiceDirectoryService, InMemorySalesInvoiceDirectoryService>();
builder.Services.AddScoped<IPurchaseInvoiceDirectoryService, InMemoryPurchaseInvoiceDirectoryService>();
builder.Services.AddSingleton<ISettingsDirectoryService, InMemorySettingsDirectoryService>();
builder.Services.AddSingleton<IElectronicDocumentIntegration, StubElectronicDocumentIntegration>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    DatabaseSeeder.EnsureSeedData(dbContext);
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
