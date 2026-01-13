using SPNotifications.Application.DTOs;
using SPNotifications.Application.Interfaces;
using SPNotifications.Domain.Entities;
using SPNotifications.Domain.Interfaces;
using SPNotifications.Domain.Common;
using SPNotifications.Domain.Exceptions;

namespace SPNotifications.Application.Services
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _repository;

        public NotificationService(INotificationRepository repository)
        {
            _repository = repository;
        }

        public async Task<PagedResult<NotificationDto>> GetAllAsync(NotificationQueryDto query)
        {
            var pagedNotifications = await _repository.GetPagedAsync(
                query.Page,
                query.PageSize,
                query.Read,
                query.Type
            );

            return new PagedResult<NotificationDto>
            {
                Items = pagedNotifications.Items.Select(n => new NotificationDto
                {
                    Id = n.Id,
                    User = n.Username,
                    Message = n.Message,
                    Type = n.Type,
                    Read = n.Read,
                    CreatedAt = n.CreatedAt
                }).ToList(),

                TotalCount = pagedNotifications.TotalCount
            };
        }

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

        public async Task MarkAsReadAsync(Guid id)
        {
            var notification = await _repository.GetByIdAsync(id);

            if (notification == null)
                throw new NotFoundException("Notificação não encontrada");

            notification.Read = true;

            await _repository.UpdateAsync(notification);
        }
    }
}
