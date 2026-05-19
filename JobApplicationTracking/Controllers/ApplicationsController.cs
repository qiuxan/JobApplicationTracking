using System.Text;
using JobApplicationTracking.Application.DTOs;
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
        await service.CreateAsync(vm.Company, vm.Role, vm.AppliedDate, vm.JobUrl, vm.ContactName, vm.ContactEmail, vm.Description, vm.Notes);
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
            Description = dto.Description,
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
        var updated = await service.UpdateAsync(id, vm.Company, vm.Role, vm.AppliedDate, vm.JobUrl, vm.ContactName, vm.ContactEmail, vm.Description, vm.Notes);
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

    [HttpGet]
    public async Task<IActionResult> ExportCsv()
    {
        var applications = await service.GetAllAsync();
        var csv = BuildCsv(applications);
        var bytes = Encoding.UTF8.GetBytes(csv);
        var filename = $"job-applications-{DateTime.Today:yyyy-MM-dd}.csv";
        return File(bytes, "text/csv", filename);
    }

    private static string BuildCsv(IReadOnlyList<JobApplicationDto> applications)
    {
        var sb = new StringBuilder();
        sb.AppendLine("Id,Company,Role,Status,Applied Date,Job URL,Contact Name,Contact Email,Notes,Description,Created At,Updated At");
        foreach (var a in applications)
            sb.AppendLine($"{a.Id},{Csv(a.Company)},{Csv(a.Role)},{a.Status},{a.AppliedDate},{Csv(a.JobUrl)},{Csv(a.ContactName)},{Csv(a.ContactEmail)},{Csv(a.Notes)},{Csv(a.Description)},{a.CreatedAt:yyyy-MM-dd HH:mm},{a.UpdatedAt:yyyy-MM-dd HH:mm}");
        return sb.ToString();
    }

    private static string Csv(string? value)
    {
        if (string.IsNullOrEmpty(value)) return string.Empty;
        return value.Contains(',') || value.Contains('"') || value.Contains('\n')
            ? $"\"{value.Replace("\"", "\"\"")}\""
            : value;
    }
}