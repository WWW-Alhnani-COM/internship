namespace InternshipManagement.API.DTOs.Student;

public class StudentProfileDto
{
    public Guid Id { get; set; }
    public string StudentId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string Major { get; set; } = string.Empty;
    public decimal? GPA { get; set; }
    public string? AcademicLevel { get; set; }
    public string? University { get; set; }
}