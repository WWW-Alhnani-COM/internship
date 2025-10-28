using System.ComponentModel.DataAnnotations;

namespace InternshipManagement.API.DTOs.SiteSupervisor;

public class ReviewWeeklyReportDto
{
    [Required]
    public Guid ReportId { get; set; }

    [Required]
    [RegularExpression("^(Approved|Rejected)$")]
    public string Status { get; set; } = "Approved";

    public string? Feedback { get; set; }
}