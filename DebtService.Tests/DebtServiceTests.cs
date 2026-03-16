using DebtService.Dtos;
using DebtService.Models;
using DebtService.Repository;
using DebtService.Services;
using Microsoft.Extensions.Configuration;
using Moq;

namespace DebtService.Tests;

public class DebtServiceTests
{
    private readonly Mock<IRepository<Debt>> _repoMock;
    private readonly Service _service;

    public DebtServiceTests()
    {
        _repoMock = new Mock<IRepository<Debt>>();

        var configMock = new Mock<IConfiguration>();
        configMock.Setup(c => c["ServiceUrls:PaymentService"]).Returns("http://localhost:5272");

        _service = new Service(_repoMock.Object, configMock.Object);
    }

    // Test 1 : Create retourne un DebtSend avec les champs corrects
    [Fact]
    public async Task Create_ReturnsDebtSend_WithCorrectFields()
    {
        var receive = new DebtReceive
        {
            UserId = Guid.NewGuid(),
            Creditor = "Banque Postale",
            OriginalAmount = 1000,
            InterestRate = 5,
            DueDate = DateTime.UtcNow.AddYears(2),
            Type = 0,
            Status = 0,
        };

        _repoMock.Setup(r => r.CreateAsync(It.IsAny<Debt>()))
            .ReturnsAsync((Debt d) => { d.Id = 1; return d; });

        var result = await _service.Create(receive);

        Assert.NotNull(result);
        Assert.Equal("Banque Postale", result.Creditor);
        Assert.Equal(1000, result.OriginalAmount);
        Assert.Equal(1000, result.RemainingAmount); // RemainingAmount = OriginalAmount à la création
    }

    // Test 2 : GetById retourne le bon DTO quand la dette existe
    [Fact]
    public async Task GetById_ReturnsDebtSend_WhenDebtExists()
    {
        var debtId = 42;
        var userId = Guid.NewGuid();
        var debt = new Debt
        {
            Id = debtId,
            UserId = userId,
            Creditor = "Crédit Agricole",
            OriginalAmount = 5000,
            RemainingAmount = 3000,
            InterestRate = 3.5,
            DueDate = DateTime.UtcNow.AddYears(3),
            Type = 1,
            Status = 0,
        };

        _repoMock.Setup(r => r.GetByIdAsync(debtId)).ReturnsAsync(debt);

        var result = await _service.GetById(debtId);

        Assert.NotNull(result);
        Assert.Equal(debtId, result.Id);
        Assert.Equal("Crédit Agricole", result.Creditor);
        Assert.Equal(3000, result.RemainingAmount);
    }

    // Test 3 : UpdateDebtAmount réduit RemainingAmount et met à jour LastPaymentDate
    [Fact]
    public async Task UpdateDebtAmount_DecreasesRemainingAndSetsLastPaymentDate()
    {
        var debt = new Debt
        {
            Id = 10,
            UserId = Guid.NewGuid(),
            Creditor = "Société Générale",
            OriginalAmount = 2000,
            RemainingAmount = 2000,
            InterestRate = 4,
            DueDate = DateTime.UtcNow.AddYears(5),
            Type = 0,
            Status = 0,
        };

        _repoMock.Setup(r => r.GetByIdAsync(10)).ReturnsAsync(debt);
        _repoMock.Setup(r => r.UpdateAsync(It.IsAny<Debt>())).ReturnsAsync(true);

        var ok = await _service.UpdateDebtAmount(10, 500);

        Assert.True(ok);
        Assert.Equal(1500, debt.RemainingAmount);
        Assert.NotNull(debt.LastPaymentDate);
    }
}
