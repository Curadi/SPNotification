using SPNotifications.Application.DTOs;
using SPNotifications.Domain.Entities;
using SPNotifications.Domain.Interfaces;

namespace SPNotifications.Application.Services;

public class NotificationService
{
    private readonly INotificationRepository _repository;

    public NotificationService(INotificationRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<NotificationDto>> GetAllAsync(NotificationQueryDto query)
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
            Message = n.Message,
            Type = n.Type,
            Read = n.Read,
            CreatedAt = n.CreatedAt
        }).ToList();
    }

    public async Task CreateAsync(string message, string type)
    {
        var notification = new Notification
        {
            Id = Guid.NewGuid(),
            Message = message,
            Type = type
        };

        await _repository.AddAsync(notification);
    }
}
