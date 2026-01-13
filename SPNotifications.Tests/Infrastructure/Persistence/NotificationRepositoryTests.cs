using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using SPNotifications.Domain.Entities;
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

            var notification = new Notification
            {
                Username = "Sistema",
                Message = "Teste",
                Type = "info",
                CreatedAt = DateTime.UtcNow,
                Read = false
            };

            // Act
            await repository.AddAsync(notification);

            // Assert
            var saved = await context.Notifications.FirstOrDefaultAsync();
            saved.Should().NotBeNull();
            saved!.Message.Should().Be("Teste");
        }

        [Fact]
        public async Task GetPagedAsync_ShouldReturnCorrectPage()
        {
            var context = CreateContext();
            var repository = new NotificationRepository(context);

            for (int i = 1; i <= 10; i++)
            {
                context.Notifications.Add(new Notification
                {
                    Username = "User",
                    Message = $"Msg {i}",
                    Type = "info",
                    CreatedAt = DateTime.UtcNow.AddMinutes(i),
                    Read = false
                });
            }

            await context.SaveChangesAsync();

            // Act
            var result = await repository.GetPagedAsync(page: 1, pageSize: 5, read: null, type: null);

            // Assert
            result.Items.Should().HaveCount(5);
            result.TotalCount.Should().Be(10);
        }

        [Fact]
        public async Task GetPagedAsync_ShouldFilterByRead()
        {
            var context = CreateContext();
            var repository = new NotificationRepository(context);

            context.Notifications.AddRange(
                new Notification { Read = true, Message = "Lida", CreatedAt = DateTime.UtcNow },
                new Notification { Read = false, Message = "Nao lida", CreatedAt = DateTime.UtcNow }
            );

            await context.SaveChangesAsync();

            var result = await repository.GetPagedAsync(1, 10, read: true, type: null);

            result.Items.Should().HaveCount(1);
            result.Items.First().Read.Should().BeTrue();
        }

        [Fact]
        public async Task UpdateAsync_ShouldMarkAsRead()
        {
            var context = CreateContext();
            var repository = new NotificationRepository(context);

            var notification = new Notification
            {
                Message = "Teste",
                Read = false,
                CreatedAt = DateTime.UtcNow
            };

            context.Notifications.Add(notification);
            await context.SaveChangesAsync();

            // Act
            notification.Read = true;
            await repository.UpdateAsync(notification);

            // Assert
            var updated = await context.Notifications.FindAsync(notification.Id);
            updated!.Read.Should().BeTrue();
        }



    }
}
