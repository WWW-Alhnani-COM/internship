using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using InternshipManagement.API.DTOs.WeeklyReport;
using InternshipManagement.API.Services;

namespace InternshipManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WeeklyReportController : ControllerBase
{
    private readonly WeeklyReportService _service;

    public WeeklyReportController(WeeklyReportService service)
    {
        _service = service;
    }

    [HttpGet("internship/{internshipId}")]
    [Authorize(Policy = "StudentOnly")]
    public async Task<ActionResult<List<WeeklyReportDto>>> GetReportsForInternship(Guid internshipId)
    {
        var userId = Guid.Parse(User.FindFirst("nameid")?.Value 
            ?? User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value);

        var reports = await _service.GetWeeklyReportsForInternshipAsync(internshipId, userId);
        return Ok(reports);
    }

    [HttpPost]
    [Authorize(Policy = "StudentOnly")]
    public async Task<ActionResult<WeeklyReportDto>> CreateReport(CreateWeeklyReportDto dto)
    {
        var userId = Guid.Parse(User.FindFirst("nameid")?.Value 
            ?? User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value);

        var result = await _service.CreateWeeklyReportAsync(dto, userId);
        if (result == null) return BadRequest(new { message = "Invalid internship or week number already exists." });

        return CreatedAtAction(nameof(GetReportsForInternship), new { internshipId = result.InternshipId }, result);
    }

    [HttpPut]
    [Authorize(Policy = "StudentOnly")]
    public async Task<ActionResult> UpdateReport(UpdateWeeklyReportDto dto)
    {
        var userId = Guid.Parse(User.FindFirst("nameid")?.Value 
            ?? User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value);

        var success = await _service.UpdateWeeklyReportAsync(dto, userId);
        if (!success) return BadRequest(new { message = "Report not found or cannot be modified." });

        return Ok();
    }

    [HttpPost("{id}/submit")]
    [Authorize(Policy = "StudentOnly")]
    public async Task<ActionResult> SubmitReport(Guid id)
    {
        var userId = Guid.Parse(User.FindFirst("nameid")?.Value 
            ?? User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value);

        var success = await _service.SubmitWeeklyReportAsync(id, userId);
        if (!success) return BadRequest(new { message = "Report not found or already submitted." });

        return Ok();
    }

    [HttpPut("review")]
    [Authorize(Policy = "SupervisorOnly")] // Site or Academic
    public async Task<ActionResult> ReviewReport(ReviewWeeklyReportDto dto)
    {
        var supervisorId = Guid.Parse(User.FindFirst("nameid")?.Value 
            ?? User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value);

        var success = await _service.ReviewWeeklyReportAsync(dto, supervisorId);
        if (!success) return BadRequest(new { message = "Report not found or access denied." });

        return Ok();
    }
}