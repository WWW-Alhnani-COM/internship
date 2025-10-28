namespace InternshipManagement.API.Entities;

public class InternshipTask
{
    public Guid Id { get; set; }
    public Guid InternshipId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime DueDate { get; set; }
    public string Status { get; set; } = "Pending"; // Pending, InProgress, Completed, Overdue
    public string Priority { get; set; } = "Medium"; // Low, Medium, High
    public string? SubmissionUrl { get; set; }
    public DateTime? SubmittedAt { get; set; }
    public string? Feedback { get; set; }
    public Guid CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public Internship Internship { get; set; } = null!;
    public User Creator { get; set; } = null!;
}