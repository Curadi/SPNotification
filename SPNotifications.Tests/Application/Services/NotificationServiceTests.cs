using Moq;
using FluentAssertions;
using SPNotifications.Application.Services;
using SPNotifications.Application.DTOs;
using SPNotifications.Domain.Entities;
using SPNotifications.Domain.Common;
using SPNotifications.Domain.Queries;

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
                    n.User == "Sistema" &&
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
            var notification = new Notification(
                "Sistema",
                "Mensagem",
                "info"
            );

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
            var queryDto = new NotificationQueryDto
            {
                Page = 2,
                PageSize = 5
            };

            var notifications = Enumerable.Range(1, 5)
                .Select(i => new Notification(
                    "User",
                    $"Msg {i}",
                    "info"
                ))
                .ToList();

            var pagedResult = new PagedResult<Notification>
            {
                Items = notifications,
                TotalCount = 20
            };

            _repositoryMock
                .Setup(r => r.GetAllAsync(It.IsAny<NotificationQuery>()))
                .ReturnsAsync(pagedResult);

            // Act
            var result = await _service.GetAllAsync(queryDto);

            // Assert
            result.Items.Should().HaveCount(5);
            result.TotalCount.Should().Be(20);

            _repositoryMock.Verify(
                r => r.GetAllAsync(It.Is<NotificationQuery>(q =>
                    q.Page == 2 &&
                    q.PageSize == 5
                )),
                Times.Once
            );
        }

        [Fact]
        public async Task GetAllAsync_ShouldFilterByRead()
        {
            // Arrange
            var queryDto = new NotificationQueryDto
            {
                Read = true
            };

            var notification = new Notification(
                "Sistema",
                "Lida",
                "info"
            );
            notification.MarkAsRead();

            var pagedResult = new PagedResult<Notification>
            {
                Items = new List<Notification> { notification },
                TotalCount = 1
            };

            _repositoryMock
                .Setup(r => r.GetAllAsync(It.IsAny<NotificationQuery>()))
                .ReturnsAsync(pagedResult);

            // Act
            var result = await _service.GetAllAsync(queryDto);

            // Assert
            result.Items.Should().OnlyContain(n => n.Read);
        }

        [Fact]
        public async Task GetAllAsync_ShouldFilterByType()
        {
            // Arrange
            var queryDto = new NotificationQueryDto
            {
                Type = "warning"
            };

            var pagedResult = new PagedResult<Notification>
            {
                Items = new List<Notification>
                {
                    new Notification("Sistema", "Aviso", "warning")
                },
                TotalCount = 1
            };

            _repositoryMock
                .Setup(r => r.GetAllAsync(It.IsAny<NotificationQuery>()))
                .ReturnsAsync(pagedResult);

            // Act
            var result = await _service.GetAllAsync(queryDto);

            // Assert
            result.Items.Should().OnlyContain(n => n.Type == "warning");
        }

        [Fact]
        public async Task GetAllAsync_ShouldApplyAllFilters()
        {
            // Arrange
            var queryDto = new NotificationQueryDto
            {
                Page = 1,
                PageSize = 10,
                Read = false,
                Type = "info"
            };

            var pagedResult = new PagedResult<Notification>
            {
                Items = new List<Notification>
                {
                    new Notification("Sistema", "Teste", "info")
                },
                TotalCount = 1
            };

            _repositoryMock
                .Setup(r => r.GetAllAsync(It.IsAny<NotificationQuery>()))
                .ReturnsAsync(pagedResult);

            // Act
            var result = await _service.GetAllAsync(queryDto);

            // Assert
            result.Items.Should().HaveCount(1);
            result.TotalCount.Should().Be(1);
        }
    }
}
