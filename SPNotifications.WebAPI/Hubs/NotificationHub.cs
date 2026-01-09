using Microsoft.AspNetCore.SignalR;
using SPNotifications.Application.DTOs;
using SPNotifications.Application.Services;

namespace SPNotifications.WebAPI.Hubs
{
    public class NotificationHub : Hub
    {
        private readonly NotificationService _service;

        public NotificationHub(NotificationService service)
        {
            _service = service;
        }

        public async Task SendMessage(string user, string message)
        {
            var dto = new CreateNotificationDto
            {
                User = user,
                Message = message
            };

            await _service.CreateAsync(dto);

            await Clients.All.SendAsync(
                "ReceiveNotification",
                user,
                message
            );
        }
    }
}
