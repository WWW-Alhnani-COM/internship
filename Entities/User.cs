namespace InternshipManagement.API.Entities;

public class User
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Role { get; set; } = "Student";
    public string? Phone { get; set; }
    public string? Avatar { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation Properties (مهم جدًا!)
    public Student? Student { get; set; }
    public Supervisor? Supervisor { get; set; }
    public ICollection<InternshipTask> CreatedTasks { get; set; } = new List<InternshipTask>();
    public ICollection<Evaluation> Evaluations { get; set; } = new List<Evaluation>();
    public ICollection<Message> SentMessages { get; set; } = new List<Message>();
    public ICollection<Message> ReceivedMessages { get; set; } = new List<Message>();
    public ICollection<ActivityLog> ActivityLogs { get; set; } = new List<ActivityLog>();
}