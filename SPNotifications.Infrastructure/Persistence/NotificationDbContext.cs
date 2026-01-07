using Microsoft.EntityFrameworkCore;
using SPNotifications.Domain.Entities;

namespace SPNotifications.Infrastructure.Persistence;

public class NotificationDbContext : DbContext
{
    public NotificationDbContext(DbContextOptions<NotificationDbContext> options)
        : base(options) { }

    public DbSet<Notification> Notifications => Set<Notification>();
}
