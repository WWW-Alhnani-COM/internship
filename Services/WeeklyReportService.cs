using Microsoft.EntityFrameworkCore;
using InternshipManagement.API.Data;
using InternshipManagement.API.Entities;
using InternshipManagement.API.DTOs.WeeklyReport;

namespace InternshipManagement.API.Services;

public class WeeklyReportService
{
    private readonly ApplicationDbContext _context;

    public WeeklyReportService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<WeeklyReportDto>> GetWeeklyReportsForInternshipAsync(Guid internshipId, Guid userId)
    {
        var reports = await _context.WeeklyReports
            .Where(r => r.InternshipId == internshipId && r.Internship.Student.UserId == userId)
            .OrderBy(r => r.WeekNumber)
            .ToListAsync();

        return reports.Select(r => new WeeklyReportDto
        {
            Id = r.Id,
            InternshipId = r.InternshipId,
            WeekNumber = r.WeekNumber,
            StartDate = r.StartDate,
            EndDate = r.EndDate,
            Achievements = r.Achievements,
            Challenges = r.Challenges,
            NextWeekPlan = r.NextWeekPlan,
            HoursWorked = r.HoursWorked,
            Status = r.Status,
            FileUrl = r.FileUrl,
            SubmittedAt = r.SubmittedAt,
            ReviewedAt = r.ReviewedAt,
            Feedback = r.Feedback,
            CreatedAt = r.CreatedAt,
            UpdatedAt = r.UpdatedAt
        }).ToList();
    }

    public async Task<WeeklyReportDto?> CreateWeeklyReportAsync(CreateWeeklyReportDto dto, Guid userId)
    {
        var internship = await _context.Internships
            .FirstOrDefaultAsync(i => i.Id == dto.InternshipId && i.Student.UserId == userId);

        if (internship == null) return null;

        // تحقق من عدم وجود تقرير بنفس الرقم للأسبوع
        if (await _context.WeeklyReports.AnyAsync(r => r.InternshipId == dto.InternshipId && r.WeekNumber == dto.WeekNumber))
            return null;

        var report = new WeeklyReport
        {
            Id = Guid.NewGuid(),
            InternshipId = dto.InternshipId,
            WeekNumber = dto.WeekNumber,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            Achievements = dto.Achievements,
            Challenges = dto.Challenges,
            NextWeekPlan = dto.NextWeekPlan,
            HoursWorked = dto.HoursWorked,
            FileUrl = dto.FileUrl,
            Status = "Draft",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.WeeklyReports.Add(report);
        await _context.SaveChangesAsync();

        return new WeeklyReportDto
        {
            Id = report.Id,
            InternshipId = report.InternshipId,
            WeekNumber = report.WeekNumber,
            StartDate = report.StartDate,
            EndDate = report.EndDate,
            Achievements = report.Achievements,
            Challenges = report.Challenges,
            NextWeekPlan = report.NextWeekPlan,
            HoursWorked = report.HoursWorked,
            Status = report.Status,
            FileUrl = report.FileUrl,
            CreatedAt = report.CreatedAt,
            UpdatedAt = report.UpdatedAt
        };
    }

    public async Task<bool> UpdateWeeklyReportAsync(UpdateWeeklyReportDto dto, Guid userId)
    {
        var report = await _context.WeeklyReports
            .Include(r => r.Internship)
                .ThenInclude(i => i.Student)
            .FirstOrDefaultAsync(r => r.Id == dto.Id && r.Internship.Student.UserId == userId && r.Status == "Draft");

        if (report == null) return false;

        report.Achievements = dto.Achievements;
        report.Challenges = dto.Challenges;
        report.NextWeekPlan = dto.NextWeekPlan;
        report.HoursWorked = dto.HoursWorked;
        report.FileUrl = dto.FileUrl;
        report.UpdatedAt = DateTime.UtcNow;

        _context.WeeklyReports.Update(report);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> SubmitWeeklyReportAsync(Guid reportId, Guid userId)
    {
        var report = await _context.WeeklyReports
            .Include(r => r.Internship)
                .ThenInclude(i => i.Student)
            .FirstOrDefaultAsync(r => r.Id == reportId && r.Internship.Student.UserId == userId && r.Status == "Draft");

        if (report == null) return false;

        report.Status = "Submitted";
        report.SubmittedAt = DateTime.UtcNow;
        report.UpdatedAt = DateTime.UtcNow;

        _context.WeeklyReports.Update(report);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ReviewWeeklyReportAsync(ReviewWeeklyReportDto dto, Guid supervisorId)
    {
        var report = await _context.WeeklyReports
            .Include(r => r.Internship)
            .FirstOrDefaultAsync(r => r.Id == dto.ReportId && 
                (r.Internship.SiteSupervisorId == supervisorId || r.Internship.AcademicSupervisorId == supervisorId));

        if (report == null) return false;

        report.Status = dto.Status;
        report.Feedback = dto.Feedback;
        report.ReviewedAt = DateTime.UtcNow;
        report.UpdatedAt = DateTime.UtcNow;

        _context.WeeklyReports.Update(report);
        await _context.SaveChangesAsync();
        return true;
    }
}