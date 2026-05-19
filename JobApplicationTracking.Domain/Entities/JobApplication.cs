namespace JobApplicationTracking.Domain.Entities;

public class JobApplication
{
    public int Id { get; private set; }
    public string Company { get; private set; } = string.Empty;
    public string Role { get; private set; } = string.Empty;
    public string? JobUrl { get; private set; }
    public string? ContactName { get; private set; }
    public string? ContactEmail { get; private set; }
    public ApplicationStatus Status { get; private set; }
    public DateOnly AppliedDate { get; private set; }
    public string? Description { get; private set; }
    public string? Notes { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    private JobApplication() { }

    public static JobApplication Create(string company, string role, DateOnly appliedDate, string? jobUrl = null, string? contactName = null, string? contactEmail = null, string? description = null, string? notes = null)
    {
        return new JobApplication
        {
            Company = company,
            Role = role,
            AppliedDate = appliedDate,
            JobUrl = jobUrl,
            ContactName = contactName,
            ContactEmail = contactEmail,
            Description = description,
            Notes = notes,
            Status = ApplicationStatus.Applied,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    public void UpdateDetails(string company, string role, DateOnly appliedDate, string? jobUrl, string? contactName, string? contactEmail, string? description, string? notes)
    {
        Company = company;
        Role = role;
        AppliedDate = appliedDate;
        JobUrl = jobUrl;
        ContactName = contactName;
        ContactEmail = contactEmail;
        Description = description;
        Notes = notes;
        UpdatedAt = DateTime.UtcNow;
    }

    public void TransitionTo(ApplicationStatus newStatus)
    {
        Status = newStatus;
        UpdatedAt = DateTime.UtcNow;
    }
}
