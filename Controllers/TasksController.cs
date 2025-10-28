using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using InternshipManagement.API.DTOs.Task;
using InternshipManagement.API.Services;

namespace InternshipManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TasksController : ControllerBase
{
    private readonly TaskService _service;

    public TasksController(TaskService service)
    {
        _service = service;
    }

    [HttpGet("internship/{internshipId}")]
    [Authorize(Policy = "StudentOnly")] // أو "SupervisorOnly" إذا أردت عرض المهام لجميع الأدوار
    public async Task<ActionResult<List<TaskDto>>> GetTasksForInternship(Guid internshipId)
    {
        var userId = Guid.Parse(User.FindFirst("nameid")?.Value 
            ?? User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value);

        var tasks = await _service.GetTasksForInternshipAsync(internshipId, userId);
        return Ok(tasks);
    }

    [HttpPost]
    [Authorize(Policy = "SupervisorOnly")] // Site or Academic
    public async Task<ActionResult<TaskDto>> CreateTask(CreateTaskDto dto)
    {
        var supervisorId = Guid.Parse(User.FindFirst("nameid")?.Value 
            ?? User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value);

        var result = await _service.CreateTaskAsync(dto, supervisorId);
        if (result == null) return BadRequest(new { message = "Invalid internship or access denied." });

        return CreatedAtAction(nameof(GetTasksForInternship), new { internshipId = result.InternshipId }, result);
    }

    [HttpPut]
    [Authorize(Policy = "SupervisorOnly")]
    public async Task<ActionResult> UpdateTask(UpdateTaskDto dto)
    {
        var supervisorId = Guid.Parse(User.FindFirst("nameid")?.Value 
            ?? User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value);

        var success = await _service.UpdateTaskAsync(dto, supervisorId);
        if (!success) return BadRequest(new { message = "Task not found or access denied." });

        return Ok();
    }

    [HttpPost("{id}/submit")]
    [Authorize(Policy = "StudentOnly")]
    public async Task<ActionResult> SubmitTask(Guid id)
    {
        var userId = Guid.Parse(User.FindFirst("nameid")?.Value 
            ?? User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value);

        var success = await _service.SubmitTaskAsync(id, userId);
        if (!success) return BadRequest(new { message = "Task not found or already completed." });

        return Ok();
    }
}