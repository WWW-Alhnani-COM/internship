namespace InternshipManagement.API.Entities;

public class Evaluation
{
    public Guid Id { get; set; }
    public Guid InternshipId { get; set; }
    public Guid EvaluatorId { get; set; }
    public string EvaluatorType { get; set; } = "SiteSupervisor"; // SiteSupervisor, AcademicSupervisor
    public int TechnicalSkills { get; set; } // 1-5
    public int Communication { get; set; } // 1-5
    public int Professionalism { get; set; } // 1-5
    public int ProblemSolving { get; set; } // 1-5
    public int Teamwork { get; set; } // 1-5
    public decimal OverallRating { get; set; }
    public string? Comments { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public Internship Internship { get; set; } = null!;
    public User Evaluator { get; set; } = null!;
}