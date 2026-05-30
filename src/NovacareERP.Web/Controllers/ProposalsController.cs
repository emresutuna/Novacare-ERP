using Microsoft.AspNetCore.Mvc;
using NovacareERP.Application.Proposals;

namespace NovacareERP.Web.Controllers;

public sealed class ProposalsController : Controller
{
    private readonly IProposalDirectoryService _proposalDirectoryService;

    public ProposalsController(IProposalDirectoryService proposalDirectoryService)
    {
        _proposalDirectoryService = proposalDirectoryService;
    }

    public IActionResult Index(string? searchTerm, bool includeCancelled = false)
    {
        var model = _proposalDirectoryService.GetList(searchTerm, includeCancelled);
        return View(model);
    }

    public IActionResult Create(Guid? customerId)
    {
        var model = _proposalDirectoryService.GetDraft(customerId);
        return View(model);
    }
}
