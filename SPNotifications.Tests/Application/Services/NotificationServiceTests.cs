using Moq;
using FluentAssertions;
using SPNotifications.Application.Services;
using SPNotifications.Application.DTOs;
using SPNotifications.Domain.Interfaces;
using SPNotifications.Domain.Entities;
using SPNotifications.Domain.Common;
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
            var dto = new CreateNotificationDto
            {
                User = "Sistema",
                Message = "Teste",
                Type = "info"
            };

            await _service.CreateAsync(dto);

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
            var notification = new Notification
            {
                Id = Guid.NewGuid(),
                Read = false
            };

            _repositoryMock
                .Setup(r => r.GetByIdAsync(notification.Id))
                .ReturnsAsync(notification);

            await _service.MarkAsReadAsync(notification.Id);

            notification.Read.Should().BeTrue();
            _repositoryMock.Verify(r => r.UpdateAsync(notification), Times.Once);
        }

        [Fact]
        public async Task GetAllAsync_ShouldApplyPagination()
        {
            var query = new NotificationQueryDto
            {
                Page = 2,
                PageSize = 5
            };

            var pagedResult = new PagedResult<Notification>
            {
                Items = Enumerable.Range(1, 5)
                    .Select(i => new Notification
                    {
                        Id = Guid.NewGuid(),
                        Username = "User",
                        Message = $"Msg {i}",
                        Type = "info",
                        Read = false,
                        CreatedAt = DateTime.UtcNow
                    })
                    .ToList(),
                TotalCount = 20
            };

            _repositoryMock
                .Setup(r => r.GetPagedAsync(2, 5, null, null))
                .ReturnsAsync(pagedResult);

            var result = await _service.GetAllAsync(query);

            result.Items.Should().HaveCount(5);
            result.TotalCount.Should().Be(20);

            _repositoryMock.Verify(
                r => r.GetPagedAsync(2, 5, null, null),
                Times.Once
            );
        }

        [Fact]
        public async Task GetAllAsync_ShouldFilterByRead()
        {
            var query = new NotificationQueryDto
            {
                Read = true
            };

            var pagedResult = new PagedResult<Notification>
            {
                Items = new List<Notification>
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
                },
                TotalCount = 1
            };

            _repositoryMock
                .Setup(r => r.GetPagedAsync(1, 10, true, null))
                .ReturnsAsync(pagedResult);

            var result = await _service.GetAllAsync(query);

            result.Items.All(n => n.Read).Should().BeTrue();
        }

        [Fact]
        public async Task GetAllAsync_ShouldFilterByType()
        {
            var query = new NotificationQueryDto
            {
                Type = "warning"
            };

            var pagedResult = new PagedResult<Notification>
            {
                Items = new List<Notification>
                {
                    new Notification
                    {
                        Type = "warning",
                        Message = "Aviso",
                        Username = "Sistema",
                        Read = false,
                        CreatedAt = DateTime.UtcNow
                    }
                },
                TotalCount = 1
            };

            _repositoryMock
                .Setup(r => r.GetPagedAsync(1, 10, null, "warning"))
                .ReturnsAsync(pagedResult);

            var result = await _service.GetAllAsync(query);

            result.Items.Should().OnlyContain(n => n.Type == "warning");
        }

        [Fact]
        public async Task GetAllAsync_ShouldApplyAllFilters()
        {
            var query = new NotificationQueryDto
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
                    new Notification
                    {
                        Type = "info",
                        Read = false,
                        Message = "Teste",
                        Username = "Sistema",
                        CreatedAt = DateTime.UtcNow
                    }
                },
                TotalCount = 1
            };

            _repositoryMock
                .Setup(r => r.GetPagedAsync(1, 10, false, "info"))
                .ReturnsAsync(pagedResult);

            var result = await _service.GetAllAsync(query);

            result.Items.Should().HaveCount(1);
            result.TotalCount.Should().Be(1);
        }
    }
}
