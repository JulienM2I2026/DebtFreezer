using DebtService.Data;
using DebtService.Dtos;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace DebtService.IntegrationTests;

/// <summary>
/// Factory qui remplace MySQL par EF Core InMemory pour les tests d'intégration.
/// Une InMemoryDatabaseRoot statique garantit que toutes les requêtes voient les mêmes données.
/// </summary>
public class DebtServiceFactory : WebApplicationFactory<Program>
{
    // Root statique = même base InMemory partagée pour toutes les instances de DbContext
    private static readonly InMemoryDatabaseRoot _dbRoot = new();
    private const string DbName = "IntegrationTestDb";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("IntegrationTest");

        // Fournir une fausse connection string pour éviter que UseMySql() lève
        // une exception "connectionString cannot be empty" avant qu'on puisse l'écraser
        builder.ConfigureAppConfiguration(config =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:default"] = "Server=localhost;Database=test;User=root;Password=test;"
            });
        });

        builder.ConfigureServices(services =>
        {
            // Retirer TOUTES les registrations liées à AppDbContext
            // (méthode robuste : cherche par ServiceType ou ImplementationType)
            var toRemove = services.Where(d =>
                d.ServiceType == typeof(DbContextOptions<AppDbContext>) ||
                d.ServiceType == typeof(AppDbContext) ||
                d.ImplementationType == typeof(AppDbContext) ||
                (d.ServiceType.IsGenericType &&
                 d.ServiceType.GetGenericTypeDefinition().FullName?
                     .Contains("IDbContextOptionsConfiguration") == true &&
                 d.ServiceType.GetGenericArguments().Length == 1 &&
                 d.ServiceType.GetGenericArguments()[0] == typeof(AppDbContext))
            ).ToList();

            foreach (var d in toRemove) services.Remove(d);

            // Remplacer par InMemory avec root partagée (garantit la persistance inter-requêtes)
            services.AddDbContext<AppDbContext>(options =>
                options.UseInMemoryDatabase(DbName, _dbRoot));
        });
    }
}

public class DebtControllerTests : IClassFixture<DebtServiceFactory>
{
    private readonly HttpClient _client;
    private static readonly JsonSerializerOptions _json = new(JsonSerializerDefaults.Web);

    public DebtControllerTests(DebtServiceFactory factory)
    {
        _client = factory.CreateClient();
    }

    private DebtReceive MakeDebtReceive(Guid? userId = null) => new()
    {
        UserId = userId ?? Guid.NewGuid(),
        Creditor = "Test Bank",
        OriginalAmount = 5000,
        RemainingAmount = 5000,
        InterestRate = 3.5,
        DueDate = DateTime.UtcNow.AddYears(2),
        Type = 0,
        Status = 0,
    };

    // Test 1 : POST /api/v1/Debt crée une dette et retourne 201
    [Fact]
    public async Task CreateDebt_Returns201_WithCreatedDebt()
    {
        var payload = MakeDebtReceive();
        var response = await _client.PostAsJsonAsync("/api/v1/Debt", payload);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var body = await response.Content.ReadAsStringAsync();
        var created = JsonSerializer.Deserialize<DebtSend>(body, _json);
        Assert.NotNull(created);
        Assert.Equal("Test Bank", created.Creditor);
        Assert.Equal(5000, created.RemainingAmount);
    }

    // Test 2 : GET /api/v1/Debt?userId=... retourne les dettes de l'utilisateur
    [Fact]
    public async Task GetAll_ReturnsDebtsForUser()
    {
        var userId = Guid.NewGuid();
        await _client.PostAsJsonAsync("/api/v1/Debt", MakeDebtReceive(userId));
        await _client.PostAsJsonAsync("/api/v1/Debt", MakeDebtReceive(userId));

        var response = await _client.GetAsync($"/api/v1/Debt?userId={userId}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadAsStringAsync();
        var debts = JsonSerializer.Deserialize<List<DebtSend>>(body, _json);
        Assert.NotNull(debts);
        Assert.Equal(2, debts.Count);
    }

    // Test 3 : GET /api/v1/Debt/{id} retourne 200 avec la bonne dette
    [Fact]
    public async Task GetById_ReturnsDebt_WhenExists()
    {
        var createResponse = await _client.PostAsJsonAsync("/api/v1/Debt", MakeDebtReceive());
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
        var created = JsonSerializer.Deserialize<DebtSend>(
            await createResponse.Content.ReadAsStringAsync(), _json)!;

        var response = await _client.GetAsync($"/api/v1/Debt/{created.Id}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadAsStringAsync();
        var debt = JsonSerializer.Deserialize<DebtSend>(body, _json);
        Assert.NotNull(debt);
        Assert.Equal(created.Id, debt.Id);
    }

    // Test 4 : GET /api/v1/Debt/paged?userId=...&page=1&pageSize=2 retourne une page de 2 sur 3
    [Fact]
    public async Task GetPaged_ReturnsPagedResult()
    {
        var userId = Guid.NewGuid();
        await _client.PostAsJsonAsync("/api/v1/Debt", MakeDebtReceive(userId));
        await _client.PostAsJsonAsync("/api/v1/Debt", MakeDebtReceive(userId));
        await _client.PostAsJsonAsync("/api/v1/Debt", MakeDebtReceive(userId));

        var response = await _client.GetAsync($"/api/v1/Debt/paged?userId={userId}&page=1&pageSize=2");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<PagedResult<DebtSend>>(body, _json);
        Assert.NotNull(result);
        Assert.Equal(3, result.TotalCount);
        Assert.Equal(2, result.Items.Count);
        Assert.Equal(2, result.TotalPages);
    }

    // Test 5 : DELETE /api/v1/Debt/{id} retourne 204 et la dette est supprimée
    [Fact]
    public async Task DeleteDebt_Returns204_AndDebtIsGone()
    {
        var createResponse = await _client.PostAsJsonAsync("/api/v1/Debt", MakeDebtReceive());
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
        var created = JsonSerializer.Deserialize<DebtSend>(
            await createResponse.Content.ReadAsStringAsync(), _json)!;

        var deleteResponse = await _client.DeleteAsync($"/api/v1/Debt/{created.Id}");
        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

        // Vérifier que la dette n'existe plus — TestHost propage l'exception serveur directement
        var ex = await Assert.ThrowsAsync<Exception>(() =>
            _client.GetAsync($"/api/v1/Debt/{created.Id}"));
        Assert.Contains("Debt not found", ex.Message);
    }
}
