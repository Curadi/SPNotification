using Microsoft.AspNetCore.SignalR;
using SPNotifications.Application.Services;

namespace SPNotifications.WebAPI.Hubs;

public class NotificationHub : Hub
{
    private readonly NotificationService _service;

    public NotificationHub(NotificationService service)
    {
        _service = service;
    }

    public async Task SendMessage(string user, string message)
    {
        await _service.CreateAsync(
            message: message,
            type: "signalr"
        );

        await Clients.All.SendAsync(
            "ReceiveNotification",
            user,
            message
        );
    }
}
