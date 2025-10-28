using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using InternshipManagement.API.DTOs.ActivityLog;
using InternshipManagement.API.Services;

namespace InternshipManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "AdminOnly")] // فقط الإداري يمكنه عرض السجلات
public class ActivityLogController : ControllerBase
{
    private readonly ActivityLogService _service;

    public ActivityLogController(ActivityLogService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<List<ActivityLogDto>>> GetActivityLogs(
        [FromQuery] Guid? userId = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var logs = await _service.GetActivityLogsAsync(userId, page, pageSize);
        return Ok(logs);
    }
}