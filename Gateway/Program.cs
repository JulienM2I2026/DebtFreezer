using Gateway.Filter;
using Gateway.Hubs;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers(options =>
{
    options.Filters.Add<TokenValidationFilter>();
});
builder.Services.AddHttpClient("AuthService", client =>
{
    client.Timeout = TimeSpan.FromSeconds(5);
});

builder.Services.AddSignalR();

builder.Services.AddCors(options =>
{
    options.AddPolicy("VitePolicy", policy =>
    {
        policy.WithOrigins("http://localhost:8080")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials(); // requis pour SignalR
    });
});

var app = builder.Build();
app.UseCors("VitePolicy");

// Configure the HTTP request pipeline.

app.UseAuthorization();

app.MapControllers();
app.MapHub<NotificationHub>("/hubs/notifications");

app.Run();



