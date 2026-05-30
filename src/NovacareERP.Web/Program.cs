using NovacareERP.Application.Dashboard;
using NovacareERP.Application.Customers;
using NovacareERP.Application.Proposals;
using NovacareERP.Application.Suppliers;
using NovacareERP.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSingleton<IDashboardSnapshotService, DashboardSnapshotService>();
builder.Services.AddSingleton<ICustomerDirectoryService, InMemoryCustomerDirectoryService>();
builder.Services.AddSingleton<ISupplierDirectoryService, InMemorySupplierDirectoryService>();
builder.Services.AddSingleton<IProposalDirectoryService, InMemoryProposalDirectoryService>();

var app = builder.Build();

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
