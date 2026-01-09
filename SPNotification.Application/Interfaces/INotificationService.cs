using SPNotifications.Application.DTOs;

namespace SPNotifications.Application.Interfaces
{
    public interface INotificationService
    {
        Task<IEnumerable<NotificationDto>> GetAllAsync(NotificationQueryDto query);
        Task CreateAsync(CreateNotificationDto dto);
        Task MarkAsReadAsync(Guid id);
    }
}
