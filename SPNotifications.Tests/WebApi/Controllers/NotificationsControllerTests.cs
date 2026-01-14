using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SPNotifications.Application.DTOs;
using SPNotifications.Application.DTOs.Common;
using SPNotifications.Application.Interfaces;
using SPNotifications.WebAPI.Controllers;

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
            var query = new NotificationQueryDto();

            var pagedResponse = new PagedResultResponse<NotificationDto>
            {
                Items = new List<NotificationDto>
            {
                new NotificationDto
                {
                    User = "Sistema",
                    Message = "Teste",
                    Type = "info",
                    Read = false
                }
            },
                TotalCount = 1,
                Page = 1,
                PageSize = 10
            };

            _serviceMock
                .Setup(s => s.GetAllAsync(query))
                .ReturnsAsync(pagedResponse);

            // Act
            var result = await _controller.GetAll(query);

            // Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.Value.Should().BeEquivalentTo(pagedResponse);
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
            var query = new NotificationQueryDto();

            var pagedResponse = new PagedResultResponse<NotificationDto>
            {
                Items = new List<NotificationDto>
            {
                new NotificationDto
                {
                    User = "Sistema",
                    Message = "Teste",
                    Type = "info",
                    Read = false
                }
            },
                TotalCount = 1,
                Page = 1,
                PageSize = 10
            };

            _serviceMock
                .Setup(s => s.GetAllAsync(query))
                .ReturnsAsync(pagedResponse);

            // Act
            var result = await _controller.GetAll(query);

            // Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.Value.Should().BeEquivalentTo(pagedResponse);
        }

        [Fact]
        public async Task GetAll_WithTypeFilter_ShouldPassCorrectQuery()
        {
            // Arrange
            var query = new NotificationQueryDto();

            var pagedResponse = new PagedResultResponse<NotificationDto>
            {
                Items = new List<NotificationDto>
            {
                new NotificationDto
                {
                    User = "Sistema",
                    Message = "Teste",
                    Type = "info",
                    Read = false
                }
            },
                TotalCount = 1,
                Page = 1,
                PageSize = 10
            };

            _serviceMock
                .Setup(s => s.GetAllAsync(query))
                .ReturnsAsync(pagedResponse);

            // Act
            var result = await _controller.GetAll(query);

            // Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.Value.Should().BeEquivalentTo(pagedResponse);
        }


        [Fact]
        public async Task GetAll_WithReadAndTypeFilter_ShouldPassCorrectQuery()
        {
            // Arrange
            var query = new NotificationQueryDto();

            var pagedResponse = new PagedResultResponse<NotificationDto>
            {
                Items = new List<NotificationDto>
            {
                new NotificationDto
                {
                    User = "Sistema",
                    Message = "Teste",
                    Type = "info",
                    Read = false
                }
            },
                TotalCount = 1,
                Page = 1,
                PageSize = 10
            };

            _serviceMock
                .Setup(s => s.GetAllAsync(query))
                .ReturnsAsync(pagedResponse);

            // Act
            var result = await _controller.GetAll(query);

            // Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.Value.Should().BeEquivalentTo(pagedResponse);
        }


        [Fact]
        public async Task GetAll_WithPaginationAndFilter_ShouldPassCorrectQuery()
        {
            // Arrange
            var query = new NotificationQueryDto();

            var pagedResponse = new PagedResultResponse<NotificationDto>
            {
                Items = new List<NotificationDto>
            {
                new NotificationDto
                {
                    User = "Sistema",
                    Message = "Teste",
                    Type = "info",
                    Read = false
                }
            },
                TotalCount = 1,
                Page = 1,
                PageSize = 10
            };

            _serviceMock
                .Setup(s => s.GetAllAsync(query))
                .ReturnsAsync(pagedResponse);

            // Act
            var result = await _controller.GetAll(query);

            // Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.Value.Should().BeEquivalentTo(pagedResponse);
        }

        [Fact]
        public async Task GetAll_WithoutFilters_ShouldUseDefaults()
        {
            // Arrange
            var query = new NotificationQueryDto();

            var pagedResponse = new PagedResultResponse<NotificationDto>
            {
                Items = new List<NotificationDto>
            {
                new NotificationDto
                {
                    User = "Sistema",
                    Message = "Teste",
                    Type = "info",
                    Read = false
                }
            },
                TotalCount = 1,
                Page = 1,
                PageSize = 10
            };

            _serviceMock
                .Setup(s => s.GetAllAsync(query))
                .ReturnsAsync(pagedResponse);

            // Act
            var result = await _controller.GetAll(query);

            // Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.Value.Should().BeEquivalentTo(pagedResponse);
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
