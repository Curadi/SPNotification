using SPNotifications.Application.DTOs;
using SPNotifications.Application.DTOs.Common;
using SPNotifications.Application.Interfaces;
using SPNotifications.Domain.Entities;
using SPNotifications.Domain.Exceptions;
using SPNotifications.Domain.Queries;

namespace SPNotifications.Application.Services
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _repository;

        public NotificationService(INotificationRepository repository)
        {
            _repository = repository;
        }

        // GET paginado + filtros
        public async Task<PagedResultResponse<NotificationDto>> GetAllAsync(
            NotificationQueryDto dto)
        {
            var query = new NotificationQuery
            {
                Page = dto.Page,
                PageSize = dto.PageSize,
                Read = dto.Read,
                Type = dto.Type
            };

            var result = await _repository.GetAllAsync(query);

            return new PagedResultResponse<NotificationDto>
            {
                Items = result.Items.Select(n => new NotificationDto
                {
                    Id = n.Id,
                    User = n.User,
                    Message = n.Message,
                    Type = n.Type,
                    Read = n.Read,
                    CreatedAt = n.CreatedAt
                }).ToList(),
                TotalCount = result.TotalCount
            };
        }

        // CREATE
        public async Task CreateAsync(CreateNotificationDto dto)
        {
            var notification = new Notification(
                dto.User,
                dto.Message,
                dto.Type
            );

            await _repository.AddAsync(notification);
        }

        // MARK AS READ
        public async Task MarkAsReadAsync(Guid id)
        {
            var notification = await _repository.GetByIdAsync(id);

            if (notification == null)
                throw new NotFoundException("Notificação não encontrada");

            notification.MarkAsRead();

            await _repository.UpdateAsync(notification);
        }
    }
}
