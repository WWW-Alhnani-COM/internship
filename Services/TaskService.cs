using Microsoft.EntityFrameworkCore;
using InternshipManagement.API.Data;
using InternshipManagement.API.Entities;
using InternshipManagement.API.DTOs.Task;

namespace InternshipManagement.API.Services;

public class TaskService
{
    private readonly ApplicationDbContext _context;

    public TaskService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<TaskDto>> GetTasksForInternshipAsync(Guid internshipId, Guid userId)
    {
        var tasks = await _context.Tasks
            .Where(t => t.InternshipId == internshipId && t.Internship.Student.UserId == userId)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();

        return tasks.Select(t => new TaskDto
        {
            Id = t.Id,
            InternshipId = t.InternshipId,
            Title = t.Title,
            Description = t.Description,
            DueDate = t.DueDate,
            Status = t.Status,
            Priority = t.Priority,
            SubmissionUrl = t.SubmissionUrl,
            SubmittedAt = t.SubmittedAt,
            Feedback = t.Feedback,
            CreatedAt = t.CreatedAt,
            UpdatedAt = t.UpdatedAt
        }).ToList();
    }

    public async Task<TaskDto?> CreateTaskAsync(CreateTaskDto dto, Guid supervisorId)
    {
        // التحقق من أن المشرف مسؤول عن هذا التدريب
        var internshipExists = await _context.Internships
            .AnyAsync(i => i.Id == dto.InternshipId && (i.SiteSupervisorId == supervisorId || i.AcademicSupervisorId == supervisorId));

        if (!internshipExists) return null;

        var task = new InternshipTask
        {
            Id = Guid.NewGuid(),
            InternshipId = dto.InternshipId,
            Title = dto.Title,
            Description = dto.Description,
            DueDate = dto.DueDate,
            Priority = dto.Priority,
            Status = "Pending",
            CreatedBy = supervisorId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Tasks.Add(task);
        await _context.SaveChangesAsync();

        return new TaskDto
        {
            Id = task.Id,
            InternshipId = task.InternshipId,
            Title = task.Title,
            Description = task.Description,
            DueDate = task.DueDate,
            Status = task.Status,
            Priority = task.Priority,
            CreatedAt = task.CreatedAt,
            UpdatedAt = task.UpdatedAt
        };
    }

    public async Task<bool> UpdateTaskAsync(UpdateTaskDto dto, Guid supervisorId)
    {
        var task = await _context.Tasks
            .Include(t => t.Internship)
            .FirstOrDefaultAsync(t => t.Id == dto.Id && 
                (t.Internship.SiteSupervisorId == supervisorId || t.Internship.AcademicSupervisorId == supervisorId));

        if (task == null) return false;

        if (!string.IsNullOrEmpty(dto.Title)) task.Title = dto.Title;
        if (!string.IsNullOrEmpty(dto.Description)) task.Description = dto.Description;
        if (dto.DueDate.HasValue) task.DueDate = dto.DueDate.Value;
        if (!string.IsNullOrEmpty(dto.Priority)) task.Priority = dto.Priority;
        if (!string.IsNullOrEmpty(dto.SubmissionUrl)) task.SubmissionUrl = dto.SubmissionUrl;
        if (!string.IsNullOrEmpty(dto.Status)) task.Status = dto.Status;

        task.UpdatedAt = DateTime.UtcNow;
        _context.Tasks.Update(task);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> SubmitTaskAsync(Guid taskId, Guid userId)
    {
        var task = await _context.Tasks
            .Include(t => t.Internship)
            .FirstOrDefaultAsync(t => t.Id == taskId && t.Internship.Student.UserId == userId && t.Status != "Completed");

        if (task == null) return false;

        task.Status = "Completed";
        task.SubmittedAt = DateTime.UtcNow;
        task.UpdatedAt = DateTime.UtcNow;

        _context.Tasks.Update(task);
        await _context.SaveChangesAsync();
        return true;
    }
}