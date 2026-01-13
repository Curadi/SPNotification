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


        [Fact]
        public async Task GetAllAsync_ShouldApplyPagination()
        {
            // Arrange
            var query = new NotificationQueryDto
            {
                Page = 2,
                PageSize = 5
            };

            var notifications = Enumerable.Range(1, 5)
                .Select(i => new Notification
                {
                    Id = Guid.NewGuid(),
                    Username = "User",
                    Message = $"Msg {i}",
                    Type = "info",
                    Read = false,
                    CreatedAt = DateTime.UtcNow
                })
                .ToList();

            _repositoryMock
                .Setup(r => r.GetPagedAsync(2, 5, null, null))
                .ReturnsAsync(notifications);

            // Act
            var result = await _service.GetAllAsync(query);

            // Assert
            result.Should().HaveCount(5);
            _repositoryMock.Verify(
                r => r.GetPagedAsync(2, 5, null, null),
                Times.Once
            );
        }

        [Fact]
        public async Task GetAllAsync_ShouldFilterByRead()
        {
            // Arrange
            var query = new NotificationQueryDto
            {
                Read = true
            };

            var notifications = new List<Notification>
        {
            new Notification
        {
            Id = Guid.NewGuid(),
            Read = true,
            Type = "info",
            Message = "Lida",
            Username = "Sistema",
            CreatedAt = DateTime.UtcNow
        }
        };

            _repositoryMock
                .Setup(r => r.GetPagedAsync(1, 10, true, null))
                .ReturnsAsync(notifications);

            // Act
            var result = await _service.GetAllAsync(query);

            // Assert
            result.All(n => n.Read).Should().BeTrue();
        }

        [Fact]
        public async Task GetAllAsync_ShouldFilterByType()
        {
            // Arrange
            var query = new NotificationQueryDto
            {
                Type = "warning"
            };

            var notifications = new List<Notification>
    {
        new Notification
        {
            Type = "warning",
            Message = "Aviso",
            Username = "Sistema",
            Read = false,
            CreatedAt = DateTime.UtcNow
        }
    };

            _repositoryMock
                .Setup(r => r.GetPagedAsync(1, 10, null, "warning"))
                .ReturnsAsync(notifications);

            // Act
            var result = await _service.GetAllAsync(query);

            // Assert
            result.Should().OnlyContain(n => n.Type == "warning");
        }

        [Fact]
        public async Task GetAllAsync_ShouldApplyAllFilters()
        {
            // Arrange
            var query = new NotificationQueryDto
            {
                Page = 1,
                PageSize = 10,
                Read = false,
                Type = "info"
            };

            var notifications = new List<Notification>
    {
        new Notification
        {
            Type = "info",
            Read = false,
            Message = "Teste",
            Username = "Sistema",
            CreatedAt = DateTime.UtcNow
        }
    };

            _repositoryMock
                .Setup(r => r.GetPagedAsync(1, 10, false, "info"))
                .ReturnsAsync(notifications);

            // Act
            var result = await _service.GetAllAsync(query);

            // Assert
            result.Should().HaveCount(1);
        }

    }
}
