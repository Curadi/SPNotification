namespace SPNotifications.Application.DTOs;

public class NotificationQueryDto
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public bool? Read { get; set; }
    public string? Type { get; set; }
}
