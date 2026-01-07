using SPNotifications.Application.DTOs;
using SPNotifications.Domain.Entities;
using SPNotifications.Domain.Interfaces;
using SPotifications.Application.DTOs;

namespace SPNotifications.Application.Services;

public class NotificationService
{
    private readonly INotificationRepository _repository;

    public NotificationService(INotificationRepository repository)
    {
        _repository = repository;
    }

    public async Task CreateAsync(CreateNotificationDto dto)
    {
        var notification = new Notification
        {
            Id = Guid.NewGuid(),
            User = dto.User,
            Message = dto.Message,
            CreatedAt = DateTime.UtcNow
        };

        await _repository.AddAsync(notification);
    }

    public async Task<List<NotificationDto>> GetAllAsync()
    {
        var notifications = await _repository.GetAllAsync();

        return notifications.Select(n => new NotificationDto
        {
            Id = n.Id,
            User = n.User,
            Message = n.Message,
            CreatedAt = n.CreatedAt
        }).ToList();
    }
}
