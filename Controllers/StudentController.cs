using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using InternshipManagement.API.DTOs.Student;
using InternshipManagement.API.Services;

namespace InternshipManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "StudentOnly")]
public class StudentController : ControllerBase
{
    private readonly StudentService _studentService;

    public StudentController(StudentService studentService)
    {
        _studentService = studentService;
    }

    [HttpGet("profile")]
    public async Task<ActionResult<StudentProfileDto>> GetProfile()
    {
        var userId = Guid.Parse(User.FindFirst("nameid")?.Value ?? User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value);
        var profile = await _studentService.GetProfileAsync(userId);
        if (profile == null)
            return NotFound(new { message = "Student profile not found." });

        return Ok(profile);
    }

    [HttpGet("internships")]
    public async Task<ActionResult<List<InternshipDto>>> GetInternships()
    {
        var userId = Guid.Parse(User.FindFirst("nameid")?.Value ?? User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value);
        var student = await _studentService.GetProfileAsync(userId);
        if (student == null)
            return NotFound();

        var internships = await _studentService.GetInternshipsAsync(student.Id);
        return Ok(internships);
    }

    [HttpPost("internships")]
    public async Task<ActionResult<InternshipDto>> CreateInternship(CreateInternshipDto dto)
    {
        var userId = Guid.Parse(User.FindFirst("nameid")?.Value ?? User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value);
        var student = await _studentService.GetProfileAsync(userId);
        if (student == null)
            return NotFound();

        var result = await _studentService.CreateInternshipAsync(dto, student.Id);
        if (result == null)
            return BadRequest(new { message = "Invalid student." });

        return CreatedAtAction(nameof(GetInternships), new { id = result.Id }, result);
    }

    [HttpPut("internships")]
    public async Task<ActionResult> UpdateInternship(UpdateInternshipDto dto)
    {
        var userId = Guid.Parse(User.FindFirst("nameid")?.Value ?? User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value);
        var student = await _studentService.GetProfileAsync(userId);
        if (student == null)
            return NotFound();

        var success = await _studentService.UpdateInternshipAsync(dto, student.Id);
        if (!success)
            return BadRequest(new { message = "Internship not found or cannot be modified." });

        return Ok();
    }

    [HttpDelete("internships/{id}")]
    public async Task<ActionResult> DeleteInternship(Guid id)
    {
        var userId = Guid.Parse(User.FindFirst("nameid")?.Value ?? User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value);
        var student = await _studentService.GetProfileAsync(userId);
        if (student == null)
            return NotFound();

        var success = await _studentService.DeleteInternshipAsync(id, student.Id);
        if (!success)
            return BadRequest(new { message = "Internship not found or cannot be deleted." });

        return Ok();
    }
}