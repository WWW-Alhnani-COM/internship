using Microsoft.EntityFrameworkCore;
using InternshipManagement.API.Data;
using InternshipManagement.API.Entities;
using InternshipManagement.API.DTOs.Student;

namespace InternshipManagement.API.Services;

public class StudentService
{
    private readonly ApplicationDbContext _context;

    public StudentService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<StudentProfileDto?> GetProfileAsync(Guid userId)
    {
        var student = await _context.Students
            .Include(s => s.User)
            .FirstOrDefaultAsync(s => s.UserId == userId);

        if (student == null) return null;

        return new StudentProfileDto
        {
            Id = student.Id,
            StudentId = student.StudentId,
            Name = student.User.Name,
            Email = student.User.Email,
            Phone = student.User.Phone,
            Major = student.Major,
            GPA = student.GPA,
            AcademicLevel = student.AcademicLevel,
            University = student.University
        };
    }

    public async Task<List<InternshipDto>> GetInternshipsAsync(Guid studentId)
    {
        var internships = await _context.Internships
            .Where(i => i.StudentId == studentId)
            .OrderByDescending(i => i.CreatedAt)
            .ToListAsync();

        return internships.Select(i => new InternshipDto
        {
            Id = i.Id,
            CompanyName = i.CompanyName,
            CompanyAddress = i.CompanyAddress,
            CompanyPhone = i.CompanyPhone,
            StartDate = i.StartDate,
            EndDate = i.EndDate,
            Status = i.Status,
            Description = i.Description,
            AgreementDocumentUrl = i.AgreementDocumentUrl,
            CreatedAt = i.CreatedAt
        }).ToList();
    }

    public async Task<InternshipDto?> CreateInternshipAsync(CreateInternshipDto dto, Guid studentId)
    {
        var studentExists = await _context.Students.AnyAsync(s => s.Id == studentId);
        if (!studentExists) return null;

        var internship = new Internship
        {
            Id = Guid.NewGuid(),
            StudentId = studentId,
            CompanyName = dto.CompanyName,
            CompanyAddress = dto.CompanyAddress,
            CompanyPhone = dto.CompanyPhone,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            Description = dto.Description,
            AgreementDocumentUrl = dto.AgreementDocumentUrl,
            Status = "PendingApproval",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Internships.Add(internship);
        await _context.SaveChangesAsync();

        return new InternshipDto
        {
            Id = internship.Id,
            CompanyName = internship.CompanyName,
            CompanyAddress = internship.CompanyAddress,
            CompanyPhone = internship.CompanyPhone,
            StartDate = internship.StartDate,
            EndDate = internship.EndDate,
            Status = internship.Status,
            Description = internship.Description,
            AgreementDocumentUrl = internship.AgreementDocumentUrl,
            CreatedAt = internship.CreatedAt
        };
    }

    public async Task<bool> UpdateInternshipAsync(UpdateInternshipDto dto, Guid studentId)
    {
        var internship = await _context.Internships
            .FirstOrDefaultAsync(i => i.Id == dto.Id && i.StudentId == studentId && i.Status == "PendingApproval");

        if (internship == null) return false;

        internship.CompanyName = dto.CompanyName;
        internship.CompanyAddress = dto.CompanyAddress;
        internship.CompanyPhone = dto.CompanyPhone;
        internship.StartDate = dto.StartDate;
        internship.EndDate = dto.EndDate;
        internship.Description = dto.Description;
        internship.AgreementDocumentUrl = dto.AgreementDocumentUrl;
        internship.UpdatedAt = DateTime.UtcNow;

        _context.Internships.Update(internship);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteInternshipAsync(Guid internshipId, Guid studentId)
    {
        var internship = await _context.Internships
            .FirstOrDefaultAsync(i => i.Id == internshipId && i.StudentId == studentId && i.Status == "PendingApproval");

        if (internship == null) return false;

        _context.Internships.Remove(internship);
        await _context.SaveChangesAsync();
        return true;
    }
}