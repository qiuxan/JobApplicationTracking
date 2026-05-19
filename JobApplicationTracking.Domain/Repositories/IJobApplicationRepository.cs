using JobApplicationTracking.Domain.Entities;

namespace JobApplicationTracking.Domain.Repositories;

public interface IJobApplicationRepository
{
    Task<IReadOnlyList<JobApplication>> GetAllAsync(ApplicationStatus? statusFilter = null);
    Task<JobApplication?> GetByIdAsync(int id);
    Task AddAsync(JobApplication application);
    Task UpdateAsync(JobApplication application);
    Task DeleteAsync(JobApplication application);
}
