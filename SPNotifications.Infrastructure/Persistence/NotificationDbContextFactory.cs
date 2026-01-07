using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace SPNotifications.Infrastructure.Persistence;

public class NotificationDbContextFactory
    : IDesignTimeDbContextFactory<NotificationDbContext>
{
    public NotificationDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<NotificationDbContext>();

        optionsBuilder.UseSqlServer(
             "Server=(localdb)\\MSSQLLocalDB;Database=SPNotificationsDB;Trusted_Connection=True;"
        );

        return new NotificationDbContext(optionsBuilder.Options);
    }
}
