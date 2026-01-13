using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SPNotifications.Application.DTOs;
using SPNotifications.Application.Interfaces;
using SPNotifications.Domain.Common;
using SPNotifications.WebAPI.Controllers;
using Xunit;

namespace SPNotifications.Tests.WebAPI.Controllers
{
    public class NotificationsControllerTests
    {
        private readonly Mock<INotificationService> _serviceMock;
        private readonly NotificationsController _controller;

        public NotificationsControllerTests()
        {
            _serviceMock = new Mock<INotificationService>();
            _controller = new NotificationsController(_serviceMock.Object);
        }

        [Fact]
        public async Task GetAll_ShouldReturnOkWithPagedResult()
        {
            // Arrange
            var query = new NotificationQueryDto();

            var pagedResult = new PagedResult<NotificationDto>
            {
                Items = new List<NotificationDto>
                {
                    new NotificationDto
                    {
                        Message = "Teste",
                        User = "Sistema",
                        Type = "info",
                        Read = false
                    }
                },
                TotalCount = 1
            };

            _serviceMock
                .Setup(s => s.GetAllAsync(query))
                .ReturnsAsync(pagedResult);

            // Act
            var result = await _controller.GetAll(query);

            // Assert
            result.Should().BeOfType<OkObjectResult>();

            var okResult = result as OkObjectResult;

            okResult!.Value.Should().BeEquivalentTo(pagedResult);
        }

        [Fact]
        public async Task Create_ShouldReturnOk()
        {
            // Arrange
            var dto = new CreateNotificationDto
            {
                User = "Sistema",
                Message = "Nova notificação",
                Type = "info"
            };

            _serviceMock
                .Setup(s => s.CreateAsync(dto))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Create(dto);

            // Assert
            result.Should().BeOfType<OkResult>();

            _serviceMock.Verify(
                s => s.CreateAsync(dto),
                Times.Once
            );
        }

        [Fact]
        public async Task MarkAsRead_ShouldReturnNoContent()
        {
            // Arrange
            var id = Guid.NewGuid();

            _serviceMock
                .Setup(s => s.MarkAsReadAsync(id))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.MarkAsRead(id);

            // Assert
            result.Should().BeOfType<NoContentResult>();

            _serviceMock.Verify(
                s => s.MarkAsReadAsync(id),
                Times.Once
            );
        }
    }
}
