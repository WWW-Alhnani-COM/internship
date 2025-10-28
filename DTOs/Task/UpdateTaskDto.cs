using System.ComponentModel.DataAnnotations;

namespace InternshipManagement.API.DTOs.Task;

public class UpdateTaskDto
{
    [Required]
    public Guid Id { get; set; }

    public string? Title { get; set; }
    public string? Description { get; set; }
    public DateTime? DueDate { get; set; }
    public string? Priority { get; set; }
    public string? SubmissionUrl { get; set; }
    public string? Status { get; set; } // "Pending", "InProgress", "Completed", "Overdue"
}