using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using InternshipManagement.API.DTOs.Evaluation;
using InternshipManagement.API.Services;

namespace InternshipManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EvaluationController : ControllerBase
{
    private readonly EvaluationService _service;

    public EvaluationController(EvaluationService service)
    {
        _service = service;
    }

    [HttpGet("internship/{internshipId}")]
    [Authorize(Policy = "StudentOnly")] // أو "SupervisorOnly" إذا أردت عرض التقييمات للمشرفين أيضًا
    public async Task<ActionResult<List<EvaluationDto>>> GetEvaluationsForInternship(Guid internshipId)
    {
        var userId = Guid.Parse(User.FindFirst("nameid")?.Value 
            ?? User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value);

        var evaluations = await _service.GetEvaluationsForInternshipAsync(internshipId, userId);
        return Ok(evaluations);
    }

    [HttpPost]
    [Authorize(Policy = "SupervisorOnly")] // Site or Academic
    public async Task<ActionResult<EvaluationDto>> CreateEvaluation(CreateEvaluationDto dto)
    {
        var evaluatorId = Guid.Parse(User.FindFirst("nameid")?.Value 
            ?? User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value);
        
        var evaluatorType = User.FindFirst("role")?.Value ?? "SiteSupervisor"; // افترض نوع المشرف من الـ Claim

        var result = await _service.CreateEvaluationAsync(dto, evaluatorId, evaluatorType);
        if (result == null) return BadRequest(new { message = "Invalid internship or evaluation already exists." });

        return CreatedAtAction(nameof(GetEvaluationsForInternship), new { internshipId = result.InternshipId }, result);
    }

    [HttpPut]
    [Authorize(Policy = "SupervisorOnly")]
    public async Task<ActionResult> UpdateEvaluation(UpdateEvaluationDto dto)
    {
        var evaluatorId = Guid.Parse(User.FindFirst("nameid")?.Value 
            ?? User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value);

        var success = await _service.UpdateEvaluationAsync(dto, evaluatorId);
        if (!success) return BadRequest(new { message = "Evaluation not found or access denied." });

        return Ok();
    }
}