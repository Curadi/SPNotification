using SPNotifications.Application.DTOs;
using SPNotifications.Domain.Common;

namespace SPNotifications.Application.Interfaces
{
    public interface INotificationService
    {
        Task<PagedResult<NotificationDto>> GetAllAsync(NotificationQueryDto query);

        Task CreateAsync(CreateNotificationDto dto);

        Task MarkAsReadAsync(Guid id);
    }
}
