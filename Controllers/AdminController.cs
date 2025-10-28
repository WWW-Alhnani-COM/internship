using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using InternshipManagement.API.DTOs.Admin;
using InternshipManagement.API.Services;

namespace InternshipManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "AdminOnly")]
public class AdminController : ControllerBase
{
    private readonly AdminService _service;

    public AdminController(AdminService service)
    {
        _service = service;
    }

    [HttpGet("users")]
    public async Task<ActionResult<List<UserListDto>>> GetAllUsers()
    {
        var users = await _service.GetAllUsersAsync();
        return Ok(users);
    }

    [HttpPut("users/role")]
    public async Task<ActionResult> UpdateUserRole(UpdateUserRoleDto dto)
    {
        var success = await _service.UpdateUserRoleAsync(dto);
        if (!success) return BadRequest(new { message = "User not found." });
        return Ok();
    }

    [HttpPut("users/status")]
    public async Task<ActionResult> UpdateUserStatus(UpdateUserStatusDto dto)
    {
        var success = await _service.UpdateUserStatusAsync(dto);
        if (!success) return BadRequest(new { message = "User not found." });
        return Ok();
    }

    [HttpDelete("users/{id}")]
    public async Task<ActionResult> DeleteUser(Guid id)
    {
        var success = await _service.DeleteUserAsync(id);
        if (!success) return BadRequest(new { message = "User not found." });
        return Ok();
    }
}