using ChallengeService.Data;
using ChallengeService.Repository;
using ChallengeService.RestClient;
using ChallengeService.Services;
using Microsoft.EntityFrameworkCore;

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
builder.Services.AddScoped<IChallengeService, ChallengeService.Services.ChallengeService>();

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

app.Run();
