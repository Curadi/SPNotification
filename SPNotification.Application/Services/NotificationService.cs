using SPNotifications.Application.DTOs;
using SPNotifications.Application.Interfaces;
using SPNotifications.Domain.Entities;
using SPNotifications.Domain.Interfaces;

namespace SPNotifications.Application.Services
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _repository;

        public NotificationService(INotificationRepository repository)
        {
            _repository = repository;
        }

        // GET com paginação + filtros
        public async Task<IEnumerable<NotificationDto>> GetAllAsync(NotificationQueryDto query)
        {
            var notifications = await _repository.GetPagedAsync(
                query.Page,
                query.PageSize,
                query.Read,
                query.Type
            );

            return notifications.Select(n => new NotificationDto
            {
                Id = n.Id,
                User = n.Username,
                Message = n.Message,
                Type = n.Type,
                Read = n.Read,
                CreatedAt = n.CreatedAt
            });
        }

        // CREATE
        public async Task CreateAsync(CreateNotificationDto dto)
        {
            var notification = new Notification
            {
                Id = Guid.NewGuid(),
                Username = dto.User,
                Message = dto.Message,
                Type = dto.Type,
                Read = false,
                CreatedAt = DateTime.UtcNow
            };

            await _repository.AddAsync(notification);
        }

        // MARK AS READ
        public async Task MarkAsReadAsync(Guid id)
        {
            var notification = await _repository.GetByIdAsync(id);

            if (notification == null)
                throw new Exception("Notificação não encontrada");

            notification.Read = true;

            await _repository.UpdateAsync(notification);
        }
    }
}
