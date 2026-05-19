using JobApplicationTracking.Application.DTOs;
using JobApplicationTracking.Domain.Entities;
using JobApplicationTracking.Domain.Repositories;

namespace JobApplicationTracking.Application.Services;

public class JobApplicationService(IJobApplicationRepository repository)
{
    public async Task<IReadOnlyList<JobApplicationDto>> GetAllAsync(ApplicationStatus? statusFilter = null)
    {
        var applications = await repository.GetAllAsync(statusFilter);
        return applications.Select(ToDto).ToList();
    }

    public async Task<JobApplicationDto?> GetByIdAsync(int id)
    {
        var application = await repository.GetByIdAsync(id);
        return application is null ? null : ToDto(application);
    }

    public async Task<JobApplicationDto> CreateAsync(string company, string role, DateOnly appliedDate, string? jobUrl, string? contactName, string? contactEmail, string? notes)
    {
        var application = JobApplication.Create(company, role, appliedDate, jobUrl, contactName, contactEmail, notes);
        await repository.AddAsync(application);
        return ToDto(application);
    }

    public async Task<bool> UpdateAsync(int id, string company, string role, DateOnly appliedDate, string? jobUrl, string? contactName, string? contactEmail, string? notes)
    {
        var application = await repository.GetByIdAsync(id);
        if (application is null) return false;

        application.UpdateDetails(company, role, appliedDate, jobUrl, contactName, contactEmail, notes);
        await repository.UpdateAsync(application);
        return true;
    }

    public async Task<bool> UpdateStatusAsync(int id, ApplicationStatus newStatus)
    {
        var application = await repository.GetByIdAsync(id);
        if (application is null) return false;

        application.TransitionTo(newStatus);
        await repository.UpdateAsync(application);
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var application = await repository.GetByIdAsync(id);
        if (application is null) return false;

        await repository.DeleteAsync(application);
        return true;
    }

    private static JobApplicationDto ToDto(JobApplication a) => new(
        a.Id, a.Company, a.Role, a.JobUrl, a.ContactName, a.ContactEmail,
        a.Status, a.AppliedDate, a.Notes, a.CreatedAt, a.UpdatedAt
    );
}