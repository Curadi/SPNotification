using Microsoft.AspNetCore.Mvc;
using SPNotifications.Application.Services;

namespace SPNotifications.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NotificationsController : ControllerBase
{
    private readonly NotificationService _service;

    public NotificationsController(NotificationService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var notifications = await _service.GetAllAsync();
        return Ok(notifications);
    }
}
