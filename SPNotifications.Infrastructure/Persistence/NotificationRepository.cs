using Microsoft.EntityFrameworkCore;
using SPNotifications.Domain.Common;
using SPNotifications.Domain.Entities;
using SPNotifications.Domain.Queries;


namespace SPNotifications.Infrastructure.Persistence
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly NotificationDbContext _context;

        public NotificationRepository(NotificationDbContext context)
        {
            _context = context;
        }

        public async Task<PagedResult<Notification>> GetAllAsync(NotificationQuery query)
        {
            var notifications = _context.Notifications.AsQueryable();

            if (query.Read.HasValue)
                notifications = notifications
                    .Where(n => n.Read == query.Read);

            if (!string.IsNullOrWhiteSpace(query.Type))
                notifications = notifications
                    .Where(n => n.Type == query.Type);

            var totalCount = await notifications.CountAsync();

            var items = await notifications
                .OrderByDescending(n => n.CreatedAt)
                .Skip((query.Page - 1) * query.PageSize)
                .Take(query.PageSize)
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
