using JobApplicationTracking.Domain.Entities;
using JobApplicationTracking.Domain.Repositories;
using JobApplicationTracking.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace JobApplicationTracking.Infrastructure.Repositories;

public class JobApplicationRepository(AppDbContext context) : IJobApplicationRepository
{
    public async Task<IReadOnlyList<JobApplication>> GetAllAsync(ApplicationStatus? statusFilter = null)
    {
        var query = context.JobApplications.AsQueryable();
        if (statusFilter.HasValue)
            query = query.Where(a => a.Status == statusFilter.Value);
        return await query.OrderByDescending(a => a.AppliedDate).ToListAsync();
    }

    public Task<JobApplication?> GetByIdAsync(int id) =>
        context.JobApplications.FirstOrDefaultAsync(a => a.Id == id);

    public async Task AddAsync(JobApplication application)
    {
        context.JobApplications.Add(application);
        await context.SaveChangesAsync();
    }

    public async Task UpdateAsync(JobApplication application)
    {
        context.JobApplications.Update(application);
        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(JobApplication application)
    {
        context.JobApplications.Remove(application);
        await context.SaveChangesAsync();
    }
}