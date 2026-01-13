using Moq;
using FluentAssertions;
using SPNotifications.Application.Services;
using SPNotifications.Application.DTOs;
using SPNotifications.Domain.Interfaces;
using SPNotifications.Domain.Entities;
using Xunit;

namespace SPNotifications.Tests.Application.Services
{
    public class NotificationServiceTests
    {
        private readonly Mock<INotificationRepository> _repositoryMock;
        private readonly NotificationService _service;

        public NotificationServiceTests()
        {
            _repositoryMock = new Mock<INotificationRepository>();
            _service = new NotificationService(_repositoryMock.Object);
        }

        [Fact]
        public async Task CreateAsync_ShouldCreateNotification()
        {
            // Arrange
            var dto = new CreateNotificationDto
            {
                User = "Sistema",
                Message = "Teste",
                Type = "info"
            };

            // Act
            await _service.CreateAsync(dto);

            // Assert
            _repositoryMock.Verify(
                r => r.AddAsync(It.Is<Notification>(n =>
                    n.Username == "Sistema" &&
                    n.Message == "Teste" &&
                    n.Type == "info" &&
                    n.Read == false
                )),
                Times.Once
            );
        }

        [Fact]
        public async Task MarkAsReadAsync_ShouldMarkNotificationAsRead()
        {
            // Arrange
            var notification = new Notification
            {
                Id = Guid.NewGuid(),
                Read = false
            };

            _repositoryMock
                .Setup(r => r.GetByIdAsync(notification.Id))
                .ReturnsAsync(notification);

            // Act
            await _service.MarkAsReadAsync(notification.Id);

            // Assert
            notification.Read.Should().BeTrue();
            _repositoryMock.Verify(r => r.UpdateAsync(notification), Times.Once);
        }

    }
}
