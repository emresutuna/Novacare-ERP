var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapGet("/api/health", () => Results.Ok(new
{
    service = "Novacare ERP API",
    status = "Healthy",
    checkedAt = DateTimeOffset.UtcNow
}))
.WithName("Health");

app.MapGet("/api/dashboard/summary", () => Results.Ok(new
{
    monthlyIncome = 184250,
    monthlyExpense = 67240,
    openInvoiceTotal = 43800,
    cashBalance = 129560,
    openInvoiceCount = 12,
    customerCount = 86
}))
.WithName("DashboardSummary");

app.Run();
