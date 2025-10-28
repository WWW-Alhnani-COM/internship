namespace InternshipManagement.API.Entities;

public class Internship
{
    public Guid Id { get; set; }
    public Guid StudentId { get; set; }
    public string CompanyName { get; set; } = string.Empty;
    public string? CompanyAddress { get; set; }
    public string? CompanyPhone { get; set; }
    public Guid? SiteSupervisorId { get; set; }
    public Guid? AcademicSupervisorId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Status { get; set; } = "PendingApproval"; // PendingApproval, Approved, InProgress, Completed, Rejected
    public string? Description { get; set; }
    public string? AgreementDocumentUrl { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public Student Student { get; set; } = null!;
    public Supervisor? SiteSupervisor { get; set; }
    public Supervisor? AcademicSupervisor { get; set; }
    public ICollection<InternshipTask> Tasks { get; set; } = new List<InternshipTask>();
    public ICollection<WeeklyReport> WeeklyReports { get; set; } = new List<WeeklyReport>();
    public ICollection<Evaluation> Evaluations { get; set; } = new List<Evaluation>();
}