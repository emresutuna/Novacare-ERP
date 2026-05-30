using Microsoft.AspNetCore.Mvc;
using NovacareERP.Application.Settings;

namespace NovacareERP.Web.Controllers;

public sealed class SettingsController : Controller
{
    private readonly ISettingsDirectoryService _settingsDirectoryService;

    public SettingsController(ISettingsDirectoryService settingsDirectoryService)
    {
        _settingsDirectoryService = settingsDirectoryService;
    }

    public IActionResult Users(string? searchTerm)
    {
        var model = _settingsDirectoryService.GetWorkspace();
        ViewBag.SearchTerm = searchTerm;
        return View(model);
    }

    public IActionResult EditUser(Guid? id)
    {
        var workspace = _settingsDirectoryService.GetWorkspace();
        var model = id is null ? workspace.Users.FirstOrDefault() : _settingsDirectoryService.GetUser(id.Value);

        if (model is null)
        {
            return NotFound();
        }

        return View(model);
    }

    public IActionResult Definitions()
    {
        return View(_settingsDirectoryService.GetWorkspace());
    }

    public IActionResult EFatura()
    {
        return View("Integration", _settingsDirectoryService.GetWorkspace().Integrations[0]);
    }

    public IActionResult Pos()
    {
        return View("Integration", _settingsDirectoryService.GetWorkspace().Integrations[1]);
    }

    public IActionResult InvoiceSettings()
    {
        return View("TemplateSettings", _settingsDirectoryService.GetWorkspace().Templates[0]);
    }

    public IActionResult ProposalTemplates()
    {
        return View("TemplateSettings", _settingsDirectoryService.GetWorkspace().Templates[1]);
    }

    public IActionResult LabelTemplates()
    {
        return View("TemplateSettings", _settingsDirectoryService.GetWorkspace().Templates[2]);
    }

    public IActionResult Sms()
    {
        return View("Integration", _settingsDirectoryService.GetWorkspace().Integrations[3]);
    }

    public IActionResult Cargo()
    {
        return View("Integration", _settingsDirectoryService.GetWorkspace().Integrations[2]);
    }
}
