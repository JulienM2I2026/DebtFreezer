using AuthentificationService.Data;
using AuthentificationService.Filter;
using AuthentificationService.Repository;
using AuthentificationService.Services;
using DebtService.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// -------------------------
// 1) Controllers + Filters
// -------------------------
builder.Services.AddControllers(options =>
{
    // Filter global : transforme les exceptions non gérées en réponse JSON (ProblemDetails)
    options.Filters.Add<ApiExceptionFilter>();

});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// -------------------------
// 2) EF Core (SQL Server)
// -------------------------
builder.Services.AddDbContext<UserDbContext>(options =>
{
    var cs = builder.Configuration.GetConnectionString("DefaultConnection")
             ?? throw new InvalidOperationException("ConnectionStrings:DefaultConnection missing");

    // Auto-detect la version du serveur MySQL/MariaDB
    options.UseMySql(cs, ServerVersion.AutoDetect(cs));
});

// -------------------------
// 3) Services (Auth)
// -------------------------
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();
builder.Services.AddScoped<IUserService, UserService>();

// -------------------------
// 4) Repositories
// -------------------------
// Generic repository (si tu l'utilises dans d'autres parties)
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

// User repository spécifique (GetByEmail, ExistsByEmail, etc.)
builder.Services.AddScoped<IUserRepository, UserRepository>();

// -------------------------
// 5) JWT Authentication
// -------------------------
var jwtKey = builder.Configuration["Jwt:Key"];
var issuer = builder.Configuration["Jwt:Issuer"];
var audience = builder.Configuration["Jwt:Audience"];

if (string.IsNullOrWhiteSpace(jwtKey) ||
    string.IsNullOrWhiteSpace(issuer) ||
    string.IsNullOrWhiteSpace(audience))
{
    throw new InvalidOperationException("JWT settings missing. Check appsettings.json: Jwt:Key, Jwt:Issuer, Jwt:Audience");
}

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            // Signature
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),

            // Issuer / Audience
            ValidateIssuer = true,
            ValidIssuer = issuer,

            ValidateAudience = true,
            ValidAudience = audience,

            // Expiration
            ValidateLifetime = true,

            // Tolérance de décalage d'horloge (microservices / machines différentes)
            ClockSkew = TimeSpan.FromMinutes(1)
        };
    });

builder.Services.AddCors(options =>
{
    options.AddPolicy("VitePolicy", policy =>
    {
        policy.WithOrigins("http://localhost:8080", "http://localhost:5099")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddAuthorization();

var app = builder.Build();
app.UseCors("VitePolicy");

// Swagger en dev
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Configure the HTTP request pipeline.
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<UserDbContext>();
    db.Database.Migrate();  // ← applique toutes les migrations EF Core automatiquement
}
app.Run();

