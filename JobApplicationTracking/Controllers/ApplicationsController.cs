using JobApplicationTracking.Application.Services;
using JobApplicationTracking.Domain.Entities;
using JobApplicationTracking.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace JobApplicationTracking.Web.Controllers;

public class ApplicationsController(JobApplicationService service) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index(ApplicationStatus? status)
    {
        var applications = await service.GetAllAsync(status);
        ViewBag.StatusFilter = status;
        return View(applications);
    }

    [HttpGet]
    public IActionResult Create() => View(new JobApplicationFormViewModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(JobApplicationFormViewModel vm)
    {
        if (!ModelState.IsValid) return View(vm);
        await service.CreateAsync(vm.Company, vm.Role, vm.AppliedDate, vm.JobUrl, vm.ContactName, vm.ContactEmail, vm.Notes);
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var dto = await service.GetByIdAsync(id);
        if (dto is null) return NotFound();
        var vm = new JobApplicationFormViewModel
        {
            Company = dto.Company,
            Role = dto.Role,
            AppliedDate = dto.AppliedDate,
            JobUrl = dto.JobUrl,
            ContactName = dto.ContactName,
            ContactEmail = dto.ContactEmail,
            Notes = dto.Notes,
            Status = dto.Status
        };
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, JobApplicationFormViewModel vm)
    {
        if (!ModelState.IsValid) return View(vm);
        var updated = await service.UpdateAsync(id, vm.Company, vm.Role, vm.AppliedDate, vm.JobUrl, vm.ContactName, vm.ContactEmail, vm.Notes);
        if (!updated) return NotFound();
        if (vm.Status.HasValue)
            await service.UpdateStatusAsync(id, vm.Status.Value);
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var dto = await service.GetByIdAsync(id);
        if (dto is null) return NotFound();
        return View(dto);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        await service.DeleteAsync(id);
        return RedirectToAction(nameof(Index));
    }
}