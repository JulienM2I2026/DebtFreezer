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
        new MySqlServerVersion(new Version(8, 0, 0))
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


app.UseAuthorization();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var retries = 10;
    while (retries-- > 0)
    {
        try { db.Database.Migrate(); break; }
        catch { if (retries == 0) throw; Thread.Sleep(3000); }
    }
}
app.Run();
