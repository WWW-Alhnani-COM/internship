using Microsoft.EntityFrameworkCore;
using InternshipManagement.API.Data;
using InternshipManagement.API.Entities;
using InternshipManagement.API.DTOs.AcademicSupervisor;

namespace InternshipManagement.API.Services;

public class AcademicSupervisorService
{
    private readonly ApplicationDbContext _context;

    public AcademicSupervisorService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<InternshipForAcademicSupervisorDto>> GetAssignedInternshipsAsync(Guid supervisorId)
    {
        var internships = await _context.Internships
            .Include(i => i.Student)
                .ThenInclude(s => s.User)
            .Where(i => i.AcademicSupervisorId == supervisorId)
            .OrderByDescending(i => i.CreatedAt)
            .ToListAsync();

        return internships.Select(i => new InternshipForAcademicSupervisorDto
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
            .FirstOrDefaultAsync(i => i.Id == dto.InternshipId && i.AcademicSupervisorId == supervisorId);

        if (internship == null) return false;

        internship.Status = dto.Status;
        internship.UpdatedAt = DateTime.UtcNow;
        _context.Internships.Update(internship);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> CreateEvaluationAsync(CreateEvaluationDto dto, Guid supervisorId)
    {
        var internshipExists = await _context.Internships
            .AnyAsync(i => i.Id == dto.InternshipId && i.AcademicSupervisorId == supervisorId);

        if (!internshipExists) return false;

        var overallRating = (decimal)(dto.TechnicalSkills + dto.Communication + dto.Professionalism + dto.ProblemSolving + dto.Teamwork) / 5;

        var evaluation = new Evaluation
        {
            Id = Guid.NewGuid(),
            InternshipId = dto.InternshipId,
            EvaluatorId = supervisorId,
            EvaluatorType = "AcademicSupervisor",
            TechnicalSkills = dto.TechnicalSkills,
            Communication = dto.Communication,
            Professionalism = dto.Professionalism,
            ProblemSolving = dto.ProblemSolving,
            Teamwork = dto.Teamwork,
            OverallRating = overallRating,
            Comments = dto.Comments,
            CreatedAt = DateTime.UtcNow
        };

        _context.Evaluations.Add(evaluation);
        await _context.SaveChangesAsync();
        return true;
    }
}