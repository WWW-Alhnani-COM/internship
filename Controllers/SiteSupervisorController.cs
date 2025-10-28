using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using InternshipManagement.API.DTOs.SiteSupervisor;
using InternshipManagement.API.Services;

namespace InternshipManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "SiteSupervisorOnly")]
public class SiteSupervisorController : ControllerBase
{
    private readonly SiteSupervisorService _service;

    public SiteSupervisorController(SiteSupervisorService service)
    {
        _service = service;
    }

    [HttpGet("internships")]
    public async Task<ActionResult<List<InternshipForSupervisorDto>>> GetAssignedInternships()
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

    [HttpPost("tasks")]
    public async Task<ActionResult> CreateTask(CreateTaskDto dto)
    {
        var supervisorId = Guid.Parse(User.FindFirst("nameid")?.Value 
            ?? User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value);
        
        var success = await _service.CreateTaskAsync(dto, supervisorId);
        if (!success) return BadRequest(new { message = "Invalid internship or access denied." });
        return Ok();
    }

    [HttpPut("reports/review")]
    public async Task<ActionResult> ReviewWeeklyReport(ReviewWeeklyReportDto dto)
    {
        var supervisorId = Guid.Parse(User.FindFirst("nameid")?.Value 
            ?? User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value);
        
        var success = await _service.ReviewWeeklyReportAsync(dto, supervisorId);
        if (!success) return BadRequest(new { message = "Report not found or access denied." });
        return Ok();
    }
}