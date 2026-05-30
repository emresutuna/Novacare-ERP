using Microsoft.AspNetCore.Mvc;
using NovacareERP.Application.Appointments;
using NovacareERP.Application.Customers;

namespace NovacareERP.Web.Controllers;

public sealed class AppointmentsController : Controller
{
    private readonly IAppointmentDirectoryService _appointmentService;
    private readonly ICustomerDirectoryService _customerService;

    public AppointmentsController(
        IAppointmentDirectoryService appointmentService,
        ICustomerDirectoryService customerService)
    {
        _appointmentService = appointmentService;
        _customerService = customerService;
    }

    public IActionResult Index(string? searchTerm, DateOnly? filterDate)
    {
        var model = _appointmentService.GetList(searchTerm, filterDate);
        return View(model);
    }

    public IActionResult Details(Guid id)
    {
        var model = _appointmentService.GetById(id);
        if (model is null) return NotFound();
        return View(model);
    }

    public IActionResult Create()
    {
        var model = _appointmentService.GetList(null, null);
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(AppointmentFormViewModel form)
    {
        _appointmentService.Add(form);
        TempData["AppointmentSaved"] = "Randevu başarıyla oluşturuldu.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Delete(Guid id)
    {
        var deleted = _appointmentService.Delete(id);
        if (!deleted) return NotFound();
        
        TempData["AppointmentSaved"] = "Randevu başarıyla silindi.";
        return RedirectToAction(nameof(Index));
    }
}
