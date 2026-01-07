namespace SPNotifications.Application.DTOs;

public class NotificationDto
{
    public Guid Id { get; set; }
    public string User { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
