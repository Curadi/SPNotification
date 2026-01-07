using Microsoft.EntityFrameworkCore;
using SPNotifications.Domain.Entities;
using SPNotifications.Domain.Interfaces;

namespace SPNotifications.Infrastructure.Persistence;

public class NotificationRepository : INotificationRepository
{
    private readonly NotificationDbContext _context;

    public NotificationRepository(NotificationDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Notification notification)
    {
        _context.Notifications.Add(notification);
        await _context.SaveChangesAsync();
    }

    public async Task<List<Notification>> GetPagedAsync(
        int page,
        int pageSize,
        bool? read,
        string? type
    )
    {
        var query = _context.Notifications.AsQueryable();

        if (read.HasValue)
            query = query.Where(n => n.Read == read.Value);

        if (!string.IsNullOrEmpty(type))
            query = query.Where(n => n.Type == type);

        return await query
            .OrderByDescending(n => n.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }
}
