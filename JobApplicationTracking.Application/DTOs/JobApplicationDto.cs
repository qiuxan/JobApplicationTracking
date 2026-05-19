using JobApplicationTracking.Domain.Entities;

namespace JobApplicationTracking.Application.DTOs;

public record JobApplicationDto(
    int Id,
    string Company,
    string Role,
    string? JobUrl,
    string? ContactName,
    string? ContactEmail,
    ApplicationStatus Status,
    DateOnly AppliedDate,
    string? Notes,
    DateTime CreatedAt,
    DateTime UpdatedAt
);