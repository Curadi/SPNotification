using SPNotifications.Application.DTOs;
using SPNotifications.Application.DTOs.Common;
using SPNotifications.Domain.Common;

namespace SPNotifications.Application.Interfaces
{
    public interface INotificationService
    {
        Task<PagedResultResponse<NotificationDto>> GetAllAsync(NotificationQueryDto query);
        Task CreateAsync(CreateNotificationDto dto);
        Task MarkAsReadAsync(Guid id);
    }
}
