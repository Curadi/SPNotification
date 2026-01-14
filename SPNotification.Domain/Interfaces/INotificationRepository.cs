using SPNotifications.Domain.Common;
using SPNotifications.Domain.Entities;
using SPNotifications.Domain.Queries;

public interface INotificationRepository
{
    Task<PagedResult<Notification>> GetAllAsync(NotificationQuery query);
    Task<Notification?> GetByIdAsync(Guid id);
    Task AddAsync(Notification notification);
    Task UpdateAsync(Notification notification);
}
