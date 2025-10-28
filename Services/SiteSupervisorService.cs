using Microsoft.EntityFrameworkCore;
using InternshipManagement.API.Data;
using InternshipManagement.API.Entities;
using InternshipManagement.API.DTOs.SiteSupervisor;

namespace InternshipManagement.API.Services;

public class SiteSupervisorService
{
    private readonly ApplicationDbContext _context;

    public SiteSupervisorService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<InternshipForSupervisorDto>> GetAssignedInternshipsAsync(Guid supervisorId)
    {
        var internships = await _context.Internships
            .Include(i => i.Student)
                .ThenInclude(s => s.User)
            .Where(i => i.SiteSupervisorId == supervisorId)
            .OrderByDescending(i => i.CreatedAt)
            .ToListAsync();

        return internships.Select(i => new InternshipForSupervisorDto
        {
            Id = i.Id,
            StudentName = i.Student.User.Name,
            StudentEmail = i.Student.User.Email,
            StudentId = i.Student.StudentId,
            CompanyName = i.CompanyName,
            StartDate = i.StartDate,
            EndDate = i.EndDate,
            Status = i.Status,
            CreatedAt = i.CreatedAt
        }).ToList();
    }

    public async Task<bool> UpdateInternshipStatusAsync(UpdateInternshipStatusDto dto, Guid supervisorId)
    {
        var internship = await _context.Internships
            .FirstOrDefaultAsync(i => i.Id == dto.InternshipId && i.SiteSupervisorId == supervisorId);

        if (internship == null) return false;

        internship.Status = dto.Status;
        internship.UpdatedAt = DateTime.UtcNow;
        _context.Internships.Update(internship);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> CreateTaskAsync(CreateTaskDto dto, Guid supervisorId)
    {
        // التحقق من أن المشرف مسؤول عن هذا التدريب
        var internshipExists = await _context.Internships
            .AnyAsync(i => i.Id == dto.InternshipId && i.SiteSupervisorId == supervisorId);

        if (!internshipExists) return false;

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
        return true;
    }

    public async Task<bool> ReviewWeeklyReportAsync(ReviewWeeklyReportDto dto, Guid supervisorId)
    {
        var report = await _context.WeeklyReports
            .Include(r => r.Internship)
            .FirstOrDefaultAsync(r => r.Id == dto.ReportId && r.Internship.SiteSupervisorId == supervisorId);

        if (report == null) return false;

        report.Status = dto.Status;
        report.Feedback = dto.Feedback;
        report.ReviewedAt = DateTime.UtcNow;
        _context.WeeklyReports.Update(report);
        await _context.SaveChangesAsync();
        return true;
    }
}