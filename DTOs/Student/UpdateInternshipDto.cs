using System.ComponentModel.DataAnnotations;

namespace InternshipManagement.API.DTOs.Student;

public class UpdateInternshipDto
{
    [Required]
    public Guid Id { get; set; }

    [Required]
    public string CompanyName { get; set; } = string.Empty;

    public string? CompanyAddress { get; set; }
    public string? CompanyPhone { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string? Description { get; set; }
    public string? AgreementDocumentUrl { get; set; }
}