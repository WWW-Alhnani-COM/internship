namespace InternshipManagement.API.Entities;

public class Supervisor
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Type { get; set; } = "Site"; // "Site" or "Academic"
    public string? Organization { get; set; }
    public string? Department { get; set; }
    public string? Specialization { get; set; }

    // Navigation
    public User User { get; set; } = null!;
    public ICollection<Internship> SiteInternships { get; set; } = new List<Internship>();
    public ICollection<Internship> AcademicInternships { get; set; } = new List<Internship>();
}