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

        [Fact]
        public async Task GetAll_WithReadFilter_ShouldPassCorrectQuery()
        {
            // Arrange
            var query = new NotificationQueryDto
            {
                Read = true
            };

            var pagedResult = new PagedResult<NotificationDto>
            {
                Items = new List<NotificationDto>
        {
            new NotificationDto { Read = true }
        },
                TotalCount = 1
            };

            _serviceMock
                .Setup(s => s.GetAllAsync(It.Is<NotificationQueryDto>(q =>
                    q.Read == true &&
                    q.Type == null &&
                    q.Page == 1 &&
                    q.PageSize == 10
                )))
                .ReturnsAsync(pagedResult);

            // Act
            var result = await _controller.GetAll(query);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task GetAll_WithTypeFilter_ShouldPassCorrectQuery()
        {
            // Arrange
            var query = new NotificationQueryDto
            {
                Type = "info"
            };

            var pagedResult = new PagedResult<NotificationDto>
            {
                Items = new List<NotificationDto>
        {
            new NotificationDto { Type = "info" }
        },
                TotalCount = 1
            };

            _serviceMock
                .Setup(s => s.GetAllAsync(It.Is<NotificationQueryDto>(q =>
                    q.Type == "info" &&
                    q.Read == null
                )))
                .ReturnsAsync(pagedResult);

            // Act
            var result = await _controller.GetAll(query);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
        }


        [Fact]
        public async Task GetAll_WithReadAndTypeFilter_ShouldPassCorrectQuery()
        {
            // Arrange
            var query = new NotificationQueryDto
            {
                Read = false,
                Type = "warning"
            };

            var pagedResult = new PagedResult<NotificationDto>
            {
                Items = new List<NotificationDto>
        {
            new NotificationDto
            {
                Read = false,
                Type = "warning"
            }
        },
                TotalCount = 1
            };

            _serviceMock
                .Setup(s => s.GetAllAsync(It.Is<NotificationQueryDto>(q =>
                    q.Read == false &&
                    q.Type == "warning"
                )))
                .ReturnsAsync(pagedResult);

            // Act
            var result = await _controller.GetAll(query);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
        }


        [Fact]
        public async Task GetAll_WithPaginationAndFilter_ShouldPassCorrectQuery()
        {
            // Arrange
            var query = new NotificationQueryDto
            {
                Page = 2,
                PageSize = 5,
                Read = true
            };

            var pagedResult = new PagedResult<NotificationDto>
            {
                Items = new List<NotificationDto>(),
                TotalCount = 12
            };

            _serviceMock
                .Setup(s => s.GetAllAsync(It.Is<NotificationQueryDto>(q =>
                    q.Page == 2 &&
                    q.PageSize == 5 &&
                    q.Read == true
                )))
                .ReturnsAsync(pagedResult);

            // Act
            var result = await _controller.GetAll(query);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task GetAll_WithoutFilters_ShouldUseDefaults()
        {
            // Arrange
            var query = new NotificationQueryDto();

            var pagedResult = new PagedResult<NotificationDto>
            {
                Items = new List<NotificationDto>(),
                TotalCount = 0
            };

            _serviceMock
                .Setup(s => s.GetAllAsync(It.Is<NotificationQueryDto>(q =>
                    q.Page == 1 &&
                    q.PageSize == 10 &&
                    q.Read == null &&
                    q.Type == null
                )))
                .ReturnsAsync(pagedResult);

            // Act
            var result = await _controller.GetAll(query);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task GetAll_WhenServiceThrows_ShouldThrowException()
        {
            // Arrange
            var query = new NotificationQueryDto();

            _serviceMock
                .Setup(s => s.GetAllAsync(query))
                .ThrowsAsync(new Exception("Erro inesperado"));

            // Act
            Func<Task> act = async () => await _controller.GetAll(query);

            // Assert
            await act.Should().ThrowAsync<Exception>()
                .WithMessage("Erro inesperado");
        }

        [Fact]
        public async Task MarkAsRead_WhenNotificationNotFound_ShouldThrowException()
        {
            // Arrange
            var id = Guid.NewGuid();

            _serviceMock
                .Setup(s => s.MarkAsReadAsync(id))
                .ThrowsAsync(new Exception("Notificação não encontrada"));

            // Act
            Func<Task> act = async () => await _controller.MarkAsRead(id);

            // Assert
            await act.Should().ThrowAsync<Exception>()
                .WithMessage("Notificação não encontrada");
        }

        [Fact]
        public async Task Create_WhenServiceFails_ShouldThrowException()
        {
            // Arrange
            var dto = new CreateNotificationDto
            {
                User = "Sistema",
                Message = "Erro",
                Type = "info"
            };

            _serviceMock
                .Setup(s => s.CreateAsync(dto))
                .ThrowsAsync(new Exception("Erro ao salvar"));

            // Act
            Func<Task> act = async () => await _controller.Create(dto);

            // Assert
            await act.Should().ThrowAsync<Exception>()
                .WithMessage("Erro ao salvar");
        }

        [Fact]
        public async Task Create_WhenDtoIsNull_ShouldReturnBadRequest()
        {
            // Act
            var result = await _controller.Create(null!);

            // Assert
            result.Should().BeOfType<BadRequestResult>();
        }

        [Fact]
        public async Task Create_WhenDtoIsNull_ShouldNotCallService()
        {
            // Act
            await _controller.Create(null!);

            // Assert
            _serviceMock.Verify(
                s => s.CreateAsync(It.IsAny<CreateNotificationDto>()),
                Times.Never
            );
        }




    }
}
