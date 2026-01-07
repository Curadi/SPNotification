using SPNotifications.Application.Services;
using SPNotifications.Domain.Interfaces;
using SPNotifications.Infrastructure.Persistence;
using SPNotifications.WebAPI.Hubs;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// SignalR
builder.Services.AddSignalR();
builder.Services.AddControllers();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseAuthorization();

// Endpoint do Hub
app.MapHub<NotificationHub>("/notificationHub");

app.MapControllers();
app.Run();


builder.Services.AddDbContext<NotificationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<INotificationRepository, NotificationRepository>();
builder.Services.AddScoped<NotificationService>();
