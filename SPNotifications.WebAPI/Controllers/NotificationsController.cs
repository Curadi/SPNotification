using Microsoft.AspNetCore.Mvc;
using SPNotifications.Application.DTOs;
using SPNotifications.Application.Interfaces;

namespace SPNotifications.WebAPI.Controllers
{
    [ApiController]
    [Route("api/notifications")]
    public class NotificationsController : ControllerBase
    {
        private readonly INotificationService _service;

        public NotificationsController(INotificationService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] NotificationQueryDto query)
        {
            var result = await _service.GetAllAsync(query);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create(
            [FromBody] CreateNotificationDto dto)
        {
            if (dto is null)
                return BadRequest();

            await _service.CreateAsync(dto);
            return Ok();
        }

        [HttpPut("{id}/read")]
        public async Task<IActionResult> MarkAsRead(Guid id)
        {
            await _service.MarkAsReadAsync(id);
            return NoContent();
        }
    }
}
