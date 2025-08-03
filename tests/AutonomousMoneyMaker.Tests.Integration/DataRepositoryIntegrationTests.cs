using AutonomousMoneyMaker.Data;
using AutonomousMoneyMaker.Data.Models;
using AutonomousMoneyMaker.Data.Repositories;
using FluentAssertions;

namespace AutonomousMoneyMaker.Tests.Integration;

public class DataRepositoryIntegrationTests
{
    [Fact]
    public async Task PortfolioRepository_SaveAndLoad_ShouldPersistData()
    {
        var repository = new InMemoryPortfolioRepository();
        var portfolio = new PortfolioData
        {
            Id = "test-portfolio-1",
            TotalValue = 15000m,
            AvailableCash = 5000m,
            LastUpdated = DateTime.UtcNow,
            Investments = new List<InvestmentData>
            {
                new() { Id = "inv1", Symbol = "AAPL", Name = "Apple Inc.", Amount = 5000m, CurrentValue = 5500m, 
                        PurchaseDate = DateTime.UtcNow.AddDays(-5), InvestmentType = "Stock" },
                new() { Id = "inv2", Symbol = "SPY", Name = "S&P 500 ETF", Amount = 4500m, CurrentValue = 4600m, 
                        PurchaseDate = DateTime.UtcNow.AddDays(-3), InvestmentType = "ETF" }
            }
        };

        await repository.SavePortfolioAsync(portfolio);
        var retrievedPortfolio = await repository.LoadPortfolioAsync("test-portfolio-1");

        retrievedPortfolio.Should().NotBeNull();
        retrievedPortfolio!.Id.Should().Be(portfolio.Id);
        retrievedPortfolio.TotalValue.Should().Be(portfolio.TotalValue);
        retrievedPortfolio.AvailableCash.Should().Be(portfolio.AvailableCash);
        retrievedPortfolio.Investments.Should().HaveCount(2);
    }

    [Fact]
    public async Task InvestmentHistoryRepository_SaveAndRetrieve_ShouldWorkCorrectly()
    {
        var repository = new InMemoryInvestmentHistoryRepository();
        var investment = new InvestmentHistoryData
        {
            Id = "hist-1",
            PortfolioId = "portfolio-1",
            Symbol = "AAPL",
            Name = "Apple Inc.",
            Amount = 1000m,
            PurchasePrice = 150m,
            PurchaseDate = DateTime.UtcNow,
            InvestmentType = "Stock",
            Strategy = "Value Investing Strategy",
            Reasoning = "Strong fundamentals",
            ConfidenceScore = 0.85m
        };

        await repository.SaveInvestmentAsync(investment);
        var history = await repository.GetInvestmentHistoryAsync("portfolio-1");

        history.Should().HaveCount(1);
        history[0].Symbol.Should().Be("AAPL");
        history[0].Strategy.Should().Be("Value Investing Strategy");
    }

    [Fact]
    public async Task InvestmentHistoryRepository_GetBySymbol_ShouldFilterCorrectly()
    {
        var repository = new InMemoryInvestmentHistoryRepository();
        
        // Add multiple investments
        await repository.SaveInvestmentAsync(new InvestmentHistoryData
        {
            Id = "hist-1", PortfolioId = "portfolio-1", Symbol = "AAPL", Name = "Apple Inc.",
            Amount = 1000m, PurchasePrice = 150m, PurchaseDate = DateTime.UtcNow,
            InvestmentType = "Stock", Strategy = "Value", Reasoning = "Test", ConfidenceScore = 0.8m
        });

        await repository.SaveInvestmentAsync(new InvestmentHistoryData
        {
            Id = "hist-2", PortfolioId = "portfolio-1", Symbol = "GOOGL", Name = "Alphabet Inc.",
            Amount = 2000m, PurchasePrice = 2500m, PurchaseDate = DateTime.UtcNow,
            InvestmentType = "Stock", Strategy = "Growth", Reasoning = "Test", ConfidenceScore = 0.9m
        });

        await repository.SaveInvestmentAsync(new InvestmentHistoryData
        {
            Id = "hist-3", PortfolioId = "portfolio-2", Symbol = "AAPL", Name = "Apple Inc.",
            Amount = 1500m, PurchasePrice = 155m, PurchaseDate = DateTime.UtcNow,
            InvestmentType = "Stock", Strategy = "Value", Reasoning = "Test", ConfidenceScore = 0.75m
        });

        var aaplInvestments = await repository.GetInvestmentsBySymbolAsync("AAPL");
        
        aaplInvestments.Should().HaveCount(2);
        aaplInvestments.Should().OnlyContain(i => i.Symbol == "AAPL");
    }

    [Fact]
    public async Task PortfolioRepository_GetAllPortfolios_ShouldReturnAllSaved()
    {
        var repository = new InMemoryPortfolioRepository();
        
        var portfolio1 = new PortfolioData { Id = "p1", TotalValue = 1000m, AvailableCash = 1000m, LastUpdated = DateTime.UtcNow };
        var portfolio2 = new PortfolioData { Id = "p2", TotalValue = 2000m, AvailableCash = 2000m, LastUpdated = DateTime.UtcNow };
        var portfolio3 = new PortfolioData { Id = "p3", TotalValue = 3000m, AvailableCash = 3000m, LastUpdated = DateTime.UtcNow };

        await repository.SavePortfolioAsync(portfolio1);
        await repository.SavePortfolioAsync(portfolio2);
        await repository.SavePortfolioAsync(portfolio3);

        var allPortfolios = await repository.GetAllPortfoliosAsync();

        allPortfolios.Should().HaveCount(3);
        allPortfolios.Select(p => p.Id).Should().Contain(new[] { "p1", "p2", "p3" });
    }

    [Fact]
    public async Task PortfolioRepository_DeletePortfolio_ShouldRemoveFromStorage()
    {
        var repository = new InMemoryPortfolioRepository();
        var portfolio = new PortfolioData { Id = "delete-test", TotalValue = 1000m, AvailableCash = 1000m, LastUpdated = DateTime.UtcNow };

        await repository.SavePortfolioAsync(portfolio);
        var retrieved = await repository.LoadPortfolioAsync("delete-test");
        retrieved.Should().NotBeNull();

        await repository.DeletePortfolioAsync("delete-test");
        var deletedRetrieval = await repository.LoadPortfolioAsync("delete-test");
        deletedRetrieval.Should().BeNull();
    }
}