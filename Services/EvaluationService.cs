using Microsoft.EntityFrameworkCore;
using InternshipManagement.API.Data;
using InternshipManagement.API.Entities;
using InternshipManagement.API.DTOs.Evaluation;

namespace InternshipManagement.API.Services;

public class EvaluationService
{
    private readonly ApplicationDbContext _context;

    public EvaluationService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<EvaluationDto>> GetEvaluationsForInternshipAsync(Guid internshipId, Guid userId)
    {
        var evaluations = await _context.Evaluations
            .Where(e => e.InternshipId == internshipId && e.Internship.Student.UserId == userId)
            .OrderByDescending(e => e.CreatedAt)
            .ToListAsync();

        return evaluations.Select(e => new EvaluationDto
        {
            Id = e.Id,
            InternshipId = e.InternshipId,
            EvaluatorId = e.EvaluatorId,
            EvaluatorType = e.EvaluatorType,
            TechnicalSkills = e.TechnicalSkills,
            Communication = e.Communication,
            Professionalism = e.Professionalism,
            ProblemSolving = e.ProblemSolving,
            Teamwork = e.Teamwork,
            OverallRating = e.OverallRating,
            Comments = e.Comments,
            CreatedAt = e.CreatedAt
        }).ToList();
    }

    public async Task<EvaluationDto?> CreateEvaluationAsync(CreateEvaluationDto dto, Guid evaluatorId, string evaluatorType)
    {
        // تحقق من أن المشرف مسؤول عن هذا التدريب
        var internshipExists = await _context.Internships
            .AnyAsync(i => i.Id == dto.InternshipId && (
                (evaluatorType == "SiteSupervisor" && i.SiteSupervisorId == evaluatorId) ||
                (evaluatorType == "AcademicSupervisor" && i.AcademicSupervisorId == evaluatorId)
            ));

        if (!internshipExists) return null;

        // تحقق من عدم وجود تقييم سابق من نفس المشرف لنفس التدريب
        if (await _context.Evaluations.AnyAsync(e => 
            e.InternshipId == dto.InternshipId && e.EvaluatorId == evaluatorId))
            return null;

        var overallRating = (decimal)(dto.TechnicalSkills + dto.Communication + dto.Professionalism + dto.ProblemSolving + dto.Teamwork) / 5;

        var evaluation = new Evaluation
        {
            Id = Guid.NewGuid(),
            InternshipId = dto.InternshipId,
            EvaluatorId = evaluatorId,
            EvaluatorType = evaluatorType,
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

        return new EvaluationDto
        {
            Id = evaluation.Id,
            InternshipId = evaluation.InternshipId,
            EvaluatorId = evaluation.EvaluatorId,
            EvaluatorType = evaluation.EvaluatorType,
            TechnicalSkills = evaluation.TechnicalSkills,
            Communication = evaluation.Communication,
            Professionalism = evaluation.Professionalism,
            ProblemSolving = evaluation.ProblemSolving,
            Teamwork = evaluation.Teamwork,
            OverallRating = evaluation.OverallRating,
            Comments = evaluation.Comments,
            CreatedAt = evaluation.CreatedAt
        };
    }

    public async Task<bool> UpdateEvaluationAsync(UpdateEvaluationDto dto, Guid evaluatorId)
    {
        var evaluation = await _context.Evaluations
            .Include(e => e.Internship)
            .FirstOrDefaultAsync(e => e.Id == dto.Id && e.EvaluatorId == evaluatorId);

        if (evaluation == null) return false;

        if (dto.TechnicalSkills.HasValue) evaluation.TechnicalSkills = dto.TechnicalSkills.Value;
        if (dto.Communication.HasValue) evaluation.Communication = dto.Communication.Value;
        if (dto.Professionalism.HasValue) evaluation.Professionalism = dto.Professionalism.Value;
        if (dto.ProblemSolving.HasValue) evaluation.ProblemSolving = dto.ProblemSolving.Value;
        if (dto.Teamwork.HasValue) evaluation.Teamwork = dto.Teamwork.Value;
        if (!string.IsNullOrEmpty(dto.Comments)) evaluation.Comments = dto.Comments;

        evaluation.UpdatedAt = DateTime.UtcNow;
        _context.Evaluations.Update(evaluation);
        await _context.SaveChangesAsync();
        return true;
    }
}