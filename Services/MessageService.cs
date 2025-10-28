using Microsoft.EntityFrameworkCore;
using InternshipManagement.API.Data;
using InternshipManagement.API.Entities;
using InternshipManagement.API.DTOs.Message;

namespace InternshipManagement.API.Services;

public class MessageService
{
    private readonly ApplicationDbContext _context;

    public MessageService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<MessageDto>> GetInboxAsync(Guid userId)
    {
        var messages = await _context.Messages
            .Include(m => m.Sender)
            .Include(m => m.Receiver)
            .Where(m => m.ReceiverId == userId)
            .OrderByDescending(m => m.CreatedAt)
            .ToListAsync();

        return messages.Select(m => new MessageDto
        {
            Id = m.Id,
            SenderId = m.SenderId,
            SenderName = m.Sender.Name,
            ReceiverId = m.ReceiverId,
            ReceiverName = m.Receiver.Name,
            Subject = m.Subject,
            Content = m.Content,
            IsRead = m.IsRead,
            ReadAt = m.ReadAt,
            CreatedAt = m.CreatedAt
        }).ToList();
    }

    public async Task<List<MessageDto>> GetOutboxAsync(Guid userId)
    {
        var messages = await _context.Messages
            .Include(m => m.Sender)
            .Include(m => m.Receiver)
            .Where(m => m.SenderId == userId)
            .OrderByDescending(m => m.CreatedAt)
            .ToListAsync();

        return messages.Select(m => new MessageDto
        {
            Id = m.Id,
            SenderId = m.SenderId,
            SenderName = m.Sender.Name,
            ReceiverId = m.ReceiverId,
            ReceiverName = m.Receiver.Name,
            Subject = m.Subject,
            Content = m.Content,
            IsRead = m.IsRead,
            ReadAt = m.ReadAt,
            CreatedAt = m.CreatedAt
        }).ToList();
    }

    public async Task<bool> SendMessageAsync(CreateMessageDto dto, Guid senderId)
    {
        // تحقق من أن المستلم موجود
        var receiverExists = await _context.Users.AnyAsync(u => u.Id == dto.ReceiverId && u.IsActive);
        if (!receiverExists) return false;

        var message = new Message
        {
            Id = Guid.NewGuid(),
            SenderId = senderId,
            ReceiverId = dto.ReceiverId,
            Subject = dto.Subject,
            Content = dto.Content,
            IsRead = false,
            CreatedAt = DateTime.UtcNow
        };

        _context.Messages.Add(message);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> MarkAsReadAsync(Guid messageId, Guid userId)
    {
        var message = await _context.Messages
            .FirstOrDefaultAsync(m => m.Id == messageId && m.ReceiverId == userId && !m.IsRead);

        if (message == null) return false;

        message.IsRead = true;
        message.ReadAt = DateTime.UtcNow;
        _context.Messages.Update(message);
        await _context.SaveChangesAsync();
        return true;
    }
}