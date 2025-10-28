using Microsoft.EntityFrameworkCore;
using InternshipManagement.API.Data;
using InternshipManagement.API.Entities;
using InternshipManagement.API.DTOs.ActivityLog;

namespace InternshipManagement.API.Services;

public class ActivityLogService
{
    private readonly ApplicationDbContext _context;

    public ActivityLogService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task LogActivityAsync(Guid userId, string action, string? entityType = null, Guid? entityId = null, string? details = null, string? ipAddress = null)
    {
        var log = new ActivityLog
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Action = action,
            EntityType = entityType,
            EntityId = entityId,
            Details = details,
            IpAddress = ipAddress,
            CreatedAt = DateTime.UtcNow
        };

        _context.ActivityLogs.Add(log);
        await _context.SaveChangesAsync();
    }

    public async Task<List<ActivityLogDto>> GetActivityLogsAsync(Guid? userId = null, int page = 1, int pageSize = 10)
    {
        var query = _context.ActivityLogs
            .Include(l => l.User)
            .AsQueryable();

        if (userId.HasValue)
            query = query.Where(l => l.UserId == userId.Value);

        var logs = await query
            .OrderByDescending(l => l.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return logs.Select(l => new ActivityLogDto
        {
            Id = l.Id,
            UserId = l.UserId,
            UserName = l.User.Name,
            Action = l.Action,
            EntityType = l.EntityType,
            EntityId = l.EntityId,
            Details = l.Details,
            IpAddress = l.IpAddress,
            CreatedAt = l.CreatedAt
        }).ToList();
    }
}