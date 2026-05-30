using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using NovacareERP.Application.Dashboard;
using NovacareERP.Web.Models;

namespace NovacareERP.Web.Controllers;

public class HomeController : Controller
{
    private readonly IDashboardSnapshotService _dashboardSnapshotService;

    public HomeController(IDashboardSnapshotService dashboardSnapshotService)
    {
        _dashboardSnapshotService = dashboardSnapshotService;
    }

    public IActionResult Index()
    {
        var model = _dashboardSnapshotService.GetCustomerForm();
        ViewBag.Summary = _dashboardSnapshotService.GetSummary();
        return View(model);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
