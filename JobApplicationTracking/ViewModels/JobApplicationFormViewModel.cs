using System.ComponentModel.DataAnnotations;
using JobApplicationTracking.Domain.Entities;

namespace JobApplicationTracking.Web.ViewModels;

public class JobApplicationFormViewModel
{
    [Required, MaxLength(200)]
    public string Company { get; set; } = string.Empty;

    [Required, MaxLength(200)]
    public string Role { get; set; } = string.Empty;

    [MaxLength(500)]
    [Display(Name = "Job URL")]
    public string? JobUrl { get; set; }

    [MaxLength(200)]
    [Display(Name = "Contact Name")]
    public string? ContactName { get; set; }

    [MaxLength(200)]
    [EmailAddress]
    [Display(Name = "Contact Email")]
    public string? ContactEmail { get; set; }

    [Required]
    [Display(Name = "Date Applied")]
    public DateOnly AppliedDate { get; set; } = DateOnly.FromDateTime(DateTime.Today);

    [MaxLength(2000)]
    public string? Notes { get; set; }

    public ApplicationStatus? Status { get; set; }
}