using DebtService.Data;
using DebtService.Dtos;
using DebtService.Models;
using DebtService.Repository;
using DebtService.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

string connectionString = builder.Configuration.GetConnectionString("default");
builder.Services.AddDbContext<AppDbContext>(option => option.UseMySql(connectionString, new MySqlServerVersion(new Version(8, 0, 0))));

builder.Services.AddScoped<IRepository<Debt>, Repository>();
builder.Services.AddScoped<IService<DebtSend, DebtReceive>, Service>();


builder.Services.AddCors(options =>
{
    options.AddPolicy("VitePolicy", policy =>
    {
        policy.WithOrigins("http://localhost:5099", "http://localhost:5272")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.

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
