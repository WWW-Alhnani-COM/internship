using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using InternshipManagement.API.DTOs.AcademicSupervisor;
using InternshipManagement.API.Services;

namespace InternshipManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "AcademicSupervisorOnly")]
public class AcademicSupervisorController : ControllerBase
{
    private readonly AcademicSupervisorService _service;

    public AcademicSupervisorController(AcademicSupervisorService service)
    {
        _service = service;
    }

    [HttpGet("internships")]
    public async Task<ActionResult<List<InternshipForAcademicSupervisorDto>>> GetAssignedInternships()
    {
        var supervisorId = Guid.Parse(User.FindFirst("nameid")?.Value 
            ?? User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value);
        
        var internships = await _service.GetAssignedInternshipsAsync(supervisorId);
        return Ok(internships);
    }

    [HttpPut("internships/status")]
    public async Task<ActionResult> UpdateInternshipStatus(UpdateInternshipStatusDto dto)
    {
        var supervisorId = Guid.Parse(User.FindFirst("nameid")?.Value 
            ?? User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value);
        
        var success = await _service.UpdateInternshipStatusAsync(dto, supervisorId);
        if (!success) return BadRequest(new { message = "Internship not found or access denied." });
        return Ok();
    }

    [HttpPost("evaluations")]
    public async Task<ActionResult> CreateEvaluation(CreateEvaluationDto dto)
    {
        var supervisorId = Guid.Parse(User.FindFirst("nameid")?.Value 
            ?? User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value);
        
        var success = await _service.CreateEvaluationAsync(dto, supervisorId);
        if (!success) return BadRequest(new { message = "Invalid internship or access denied." });
        return Ok();
    }
}