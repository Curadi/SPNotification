using SPNotifications.Domain.Common;
using SPNotifications.Domain.Entities;

namespace SPNotifications.Domain.Interfaces
{
    public interface INotificationRepository
    {
        Task<PagedResult<Notification>> GetPagedAsync(
            int page,
            int pageSize,
            bool? read,
            string? type
        );

        Task<Notification?> GetByIdAsync(Guid id);

        Task AddAsync(Notification notification);

        Task UpdateAsync(Notification notification);
    }
}
