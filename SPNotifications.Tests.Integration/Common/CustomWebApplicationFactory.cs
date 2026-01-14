using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SPNotifications.Infrastructure.Persistence;

namespace SPNotifications.Tests.Integration.Common
{
    public class CustomWebApplicationFactory
        : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                // 🔥 REMOVE TUDO RELACIONADO AO EF CORE
                var efServices = services
                    .Where(s =>
                        s.ServiceType.FullName!.Contains("EntityFrameworkCore"))
                    .ToList();

                foreach (var service in efServices)
                {
                    services.Remove(service);
                }

                // 🔥 REMOVE TAMBÉM O DB CONTEXT EXPLICITAMENTE
                var dbContextDescriptors = services
                    .Where(d =>
                        d.ServiceType == typeof(DbContextOptions<NotificationDbContext>) ||
                        d.ServiceType == typeof(NotificationDbContext))
                    .ToList();

                foreach (var descriptor in dbContextDescriptors)
                {
                    services.Remove(descriptor);
                }

                // ✅ REGISTRA APENAS INMEMORY
                services.AddDbContext<NotificationDbContext>(options =>
                {
                    options.UseInMemoryDatabase("IntegrationTestsDb");
                });
            });
        }
    }
}
