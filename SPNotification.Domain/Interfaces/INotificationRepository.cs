using SPNotifications.Domain.Entities;

namespace SPNotifications.Domain.Interfaces;

public interface INotificationRepository
{
    Task AddAsync(Notification notification);

    Task<List<Notification>> GetAllAsync();
}
