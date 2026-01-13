using Microsoft.EntityFrameworkCore;
using SPNotifications.Domain.Common;
using SPNotifications.Domain.Entities;
using SPNotifications.Domain.Interfaces;

namespace SPNotifications.Infrastructure.Persistence
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly NotificationDbContext _context;

        public NotificationRepository(NotificationDbContext context)
        {
            _context = context;
        }

        public async Task<PagedResult<Notification>> GetPagedAsync(
        int page,
        int pageSize,
        bool? read,
        string? type)
        {
            var query = _context.Notifications.AsQueryable();

            if (read.HasValue)
                query = query.Where(n => n.Read == read);

            if (!string.IsNullOrEmpty(type))
                query = query.Where(n => n.Type == type);

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderByDescending(n => n.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<Notification>
            {
                Items = items,
                TotalCount = totalCount
            };
        }


        public async Task<Notification?> GetByIdAsync(Guid id)
        {
            return await _context.Notifications.FindAsync(id);
        }

        public async Task AddAsync(Notification notification)
        {
            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Notification notification)
        {
            _context.Notifications.Update(notification);
            await _context.SaveChangesAsync();
        }
    }
}
