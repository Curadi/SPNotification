using SPNotifications.Domain.Entities;

namespace SPNotifications.Domain.Interfaces;

public interface INotificationRepository
{
    Task AddAsync(Notification notification);

    Task<List<Notification>> GetPagedAsync(
        int page,
        int pageSize,
        bool? read,
        string? type
    );
}
