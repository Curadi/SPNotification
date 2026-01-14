using Microsoft.Extensions.DependencyInjection;
using SPNotifications.Infrastructure.Persistence;

namespace SPNotifications.Tests.Integration.Common
{
    public abstract class IntegrationTestBase
    {
        protected readonly HttpClient Client;

        protected IntegrationTestBase(CustomWebApplicationFactory factory)
        {
            Client = factory.CreateClient();

            using var scope = factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<NotificationDbContext>();

            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();
        }
    }
}
