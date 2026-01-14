using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using SPNotifications.Domain.Common;
using SPNotifications.Domain.Entities;
using SPNotifications.Domain.Queries;
using SPNotifications.Infrastructure.Persistence;
using Xunit;

namespace SPNotifications.Tests.Infrastructure.Persistence
{
    public class NotificationRepositoryTests
    {
        private static NotificationDbContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<NotificationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new NotificationDbContext(options);
        }

        [Fact]
        public async Task AddAsync_ShouldPersistNotification()
        {
            // Arrange
            var context = CreateContext();
            var repository = new NotificationRepository(context);

            var notification = new Notification(
                user: "Sistema",
                message: "Teste",
                type: "info"
            );

            // Act
            await repository.AddAsync(notification);

            // Assert
            var saved = await context.Notifications.FirstOrDefaultAsync();
            saved.Should().NotBeNull();
            saved!.Message.Should().Be("Teste");
            saved.User.Should().Be("Sistema");
            saved.Type.Should().Be("info");
            saved.Read.Should().BeFalse();
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnCorrectPage()
        {
            // Arrange
            var context = CreateContext();
            var repository = new NotificationRepository(context);

            for (int i = 1; i <= 10; i++)
            {
                context.Notifications.Add(
                    new Notification(
                        user: "User",
                        message: $"Msg {i}",
                        type: "info"
                    )
                );
            }

            await context.SaveChangesAsync();

            var query = new NotificationQuery
            {
                Page = 1,
                PageSize = 5
            };

            // Act
            var result = await repository.GetAllAsync(query);

            // Assert
            result.Items.Should().HaveCount(5);
            result.TotalCount.Should().Be(10);
        }

        [Fact]
        public async Task GetAllAsync_ShouldFilterByRead()
        {
            // Arrange
            var context = CreateContext();
            var repository = new NotificationRepository(context);

            var readNotification = new Notification(
                "Sistema",
                "Lida",
                "info"
            );
            readNotification.MarkAsRead();

            var unreadNotification = new Notification(
                "Sistema",
                "Não lida",
                "info"
            );

            context.Notifications.AddRange(
                readNotification,
                unreadNotification
            );

            await context.SaveChangesAsync();

            var query = new NotificationQuery
            {
                Read = true
            };

            // Act
            var result = await repository.GetAllAsync(query);

            // Assert
            result.Items.Should().HaveCount(1);
            result.Items.First().Read.Should().BeTrue();
        }

        [Fact]
        public async Task UpdateAsync_ShouldPersistDomainChanges()
        {
            // Arrange
            var context = CreateContext();
            var repository = new NotificationRepository(context);

            var notification = new Notification(
                "Sistema",
                "Teste",
                "info"
            );

            context.Notifications.Add(notification);
            await context.SaveChangesAsync();

            // Act
            notification.MarkAsRead();
            await repository.UpdateAsync(notification);

            // Assert
            var updated = await context.Notifications.FindAsync(notification.Id);
            updated!.Read.Should().BeTrue();
        }
    }
}
