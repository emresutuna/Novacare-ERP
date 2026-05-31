using Microsoft.AspNetCore.Mvc;
using NovacareERP.Application.CashManagement;

namespace NovacareERP.Web.Controllers;

public sealed class CashManagementController : Controller
{
    private readonly ICashManagementDirectoryService _cashManagementDirectoryService;

    public CashManagementController(ICashManagementDirectoryService cashManagementDirectoryService)
    {
        _cashManagementDirectoryService = cashManagementDirectoryService;
    }

    public IActionResult Index()
    {
        var model = _cashManagementDirectoryService.GetWorkspace();
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult CreateAccount(CashAccountFormViewModel form)
    {
        _cashManagementDirectoryService.AddAccount(form);
        TempData["CashAccountSaved"] = "Hesap kaydi olusturuldu.";
        return RedirectToAction(nameof(Index));
    }
}
