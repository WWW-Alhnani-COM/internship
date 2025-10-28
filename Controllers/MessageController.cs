using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using InternshipManagement.API.DTOs.Message;
using InternshipManagement.API.Services;

namespace InternshipManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MessageController : ControllerBase
{
    private readonly MessageService _service;

    public MessageController(MessageService service)
    {
        _service = service;
    }

    [HttpGet("inbox")]
    [Authorize]
    public async Task<ActionResult<List<MessageDto>>> GetInbox()
    {
        var userId = Guid.Parse(User.FindFirst("nameid")?.Value 
            ?? User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value);

        var messages = await _service.GetInboxAsync(userId);
        return Ok(messages);
    }

    [HttpGet("outbox")]
    [Authorize]
    public async Task<ActionResult<List<MessageDto>>> GetOutbox()
    {
        var userId = Guid.Parse(User.FindFirst("nameid")?.Value 
            ?? User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value);

        var messages = await _service.GetOutboxAsync(userId);
        return Ok(messages);
    }

    [HttpPost]
    [Authorize]
    public async Task<ActionResult> SendMessage(CreateMessageDto dto)
    {
        var senderId = Guid.Parse(User.FindFirst("nameid")?.Value 
            ?? User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value);

        var success = await _service.SendMessageAsync(dto, senderId);
        if (!success) return BadRequest(new { message = "Receiver not found or inactive." });

        return Ok();
    }

    [HttpPut("{id}/read")]
    [Authorize]
    public async Task<ActionResult> MarkAsRead(Guid id)
    {
        var userId = Guid.Parse(User.FindFirst("nameid")?.Value 
            ?? User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value);

        var success = await _service.MarkAsReadAsync(id, userId);
        if (!success) return BadRequest(new { message = "Message not found or already read." });

        return Ok();
    }
}