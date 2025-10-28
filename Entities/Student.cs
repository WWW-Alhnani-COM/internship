namespace InternshipManagement.API.Entities;

public class Student
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string StudentId { get; set; } = string.Empty;
    public string Major { get; set; } = string.Empty;
    public decimal? GPA { get; set; }
    public string? AcademicLevel { get; set; }
    public string? University { get; set; }

    // Navigation
    public User User { get; set; } = null!;
    public ICollection<Internship> Internships { get; set; } = new List<Internship>();
}