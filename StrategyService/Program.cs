using Microsoft.EntityFrameworkCore;
using StrategyService.Data;
using StrategyService.Models;
using StrategyService.Repository;
using StrategyService.RestClient;
using StrategyService.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// DbContext - MySQL
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(
        connectionString,
        ServerVersion.AutoDetect(connectionString)
    ));

// Repository générique
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

// Clients HTTP nommés vers les autres microservices
builder.Services.AddHttpClient("DebtService", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ServiceUrls:DebtService"]!);
    client.Timeout = TimeSpan.FromSeconds(10);
});

builder.Services.AddHttpClient("PaymentService", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ServiceUrls:PaymentService"]!);
    client.Timeout = TimeSpan.FromSeconds(10);
});

// ServiceClient (wrapper IHttpClientFactory)
builder.Services.AddScoped<ServiceClient>();

// Services
builder.Services.AddScoped<IStrategyService, StrategyService.Services.StrategyService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Configure the HTTP request pipeline.
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();  // ← applique toutes les migrations EF Core automatiquement
}
app.Run();
