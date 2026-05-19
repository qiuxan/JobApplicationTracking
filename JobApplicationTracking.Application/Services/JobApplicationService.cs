using JobApplicationTracking.Application.DTOs;
using JobApplicationTracking.Domain.Entities;
using JobApplicationTracking.Domain.Exceptions;
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

    public async Task<JobApplicationDto> CreateAsync(string company, string role, DateOnly appliedDate, string? jobUrl, string? contactName, string? contactEmail, string? description, string? notes)
    {
        if (await repository.ExistsAsync(company, role, appliedDate))
            throw new DuplicateApplicationException(company, role, appliedDate);

        var application = JobApplication.Create(company, role, appliedDate, jobUrl, contactName, contactEmail, description, notes);
        await repository.AddAsync(application);
        return ToDto(application);
    }

    public async Task<bool> UpdateAsync(int id, string company, string role, DateOnly appliedDate, string? jobUrl, string? contactName, string? contactEmail, string? description, string? notes)
    {
        var application = await repository.GetByIdAsync(id);
        if (application is null) return false;

        application.UpdateDetails(company, role, appliedDate, jobUrl, contactName, contactEmail, description, notes);
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

    public async Task<(int Imported, int Skipped, int Duplicates)> ImportCsvAsync(Stream csvStream)
    {
        using var reader = new StreamReader(csvStream);
        var content = await reader.ReadToEndAsync();
        var rows = ParseCsv(content);

        if (rows.Count < 2) return (0, 0, 0);

        // Map column names → indices so any future fields are picked up automatically
        var headers = rows[0]
            .Select((h, i) => (Key: h.Trim().ToLowerInvariant(), Index: i))
            .ToDictionary(x => x.Key, x => x.Index);

        int imported = 0, skipped = 0, duplicates = 0;

        foreach (var row in rows.Skip(1))
        {
            if (row.All(string.IsNullOrWhiteSpace)) continue;

            string Get(string name) =>
                headers.TryGetValue(name, out var idx) && idx < row.Count ? row[idx].Trim() : string.Empty;

            var company = Get("company");
            var role = Get("role");
            var appliedDateRaw = Get("applied date");

            if (string.IsNullOrWhiteSpace(company) || string.IsNullOrWhiteSpace(role)
                || !DateOnly.TryParse(appliedDateRaw, out var appliedDate))
            {
                skipped++;
                continue;
            }

            if (await repository.ExistsAsync(company, role, appliedDate))
            {
                duplicates++;
                continue;
            }

            var status = Enum.TryParse<ApplicationStatus>(Get("status"), ignoreCase: true, out var parsed)
                ? parsed : ApplicationStatus.Applied;

            var application = JobApplication.Create(
                company, role, appliedDate,
                jobUrl: Blank(Get("job url")),
                contactName: Blank(Get("contact name")),
                contactEmail: Blank(Get("contact email")),
                description: Blank(Get("description")),
                notes: Blank(Get("notes"))
            );

            if (status != ApplicationStatus.Applied)
                application.TransitionTo(status);

            await repository.AddAsync(application);
            imported++;
        }

        return (imported, skipped, duplicates);
    }

    private static string? Blank(string value) => string.IsNullOrWhiteSpace(value) ? null : value;

    // Full CSV parser — handles quoted fields containing commas, quotes, and newlines
    private static List<List<string>> ParseCsv(string content)
    {
        var rows = new List<List<string>>();
        var row = new List<string>();
        var field = new System.Text.StringBuilder();
        bool inQuotes = false;

        for (int i = 0; i < content.Length; i++)
        {
            char c = content[i];
            if (inQuotes)
            {
                if (c == '"' && i + 1 < content.Length && content[i + 1] == '"')
                { field.Append('"'); i++; }
                else if (c == '"')
                    inQuotes = false;
                else
                    field.Append(c);
            }
            else
            {
                switch (c)
                {
                    case '"': inQuotes = true; break;
                    case ',':
                        row.Add(field.ToString()); field.Clear(); break;
                    case '\r': break;
                    case '\n':
                        row.Add(field.ToString()); field.Clear();
                        rows.Add(row); row = []; break;
                    default: field.Append(c); break;
                }
            }
        }

        if (field.Length > 0 || row.Count > 0)
        { row.Add(field.ToString()); rows.Add(row); }

        return rows;
    }

    private static JobApplicationDto ToDto(JobApplication a) => new(
        a.Id, a.Company, a.Role, a.JobUrl, a.ContactName, a.ContactEmail,
        a.Status, a.AppliedDate, a.Description, a.Notes, a.CreatedAt, a.UpdatedAt
    );
}