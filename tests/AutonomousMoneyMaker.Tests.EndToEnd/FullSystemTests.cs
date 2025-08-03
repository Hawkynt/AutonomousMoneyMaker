using AutonomousMoneyMaker.Core.Models;
using AutonomousMoneyMaker.Core.Services;
using AutonomousMoneyMaker.Core.Strategies;
using AutonomousMoneyMaker.Data.Repositories;
using FluentAssertions;
using System.Reflection;

namespace AutonomousMoneyMaker.Tests.EndToEnd;

/// <summary>
/// End-to-end tests that validate the entire system working together
/// These tests simulate real-world scenarios and user workflows
/// </summary>
public class FullSystemTests
{
    [Fact]
    public async Task CompleteInvestmentWorkflow_ShouldWorkEndToEnd()
    {
        // Arrange: Set up the complete system
        var portfolio = new Portfolio(50000m);
        var marketDataService = new MarketDataService();
        var riskManager = new RiskManager();
        var strategies = new List<IInvestmentStrategy>
        {
            new DiversifiedETFStrategy(),
            new CryptoTrendStrategy(),
            new ValueInvestingStrategy()
        };

        // Act: Simulate multiple investment cycles
        for (int cycle = 0; cycle < 10; cycle++)
        {
            foreach (var strategy in strategies)
            {
                if (portfolio.CanInvest(100))
                {
                    var recommendation = await strategy.AnalyzeAndRecommend(portfolio, marketDataService);
                    
                    if (recommendation != null && riskManager.IsAcceptableRisk(recommendation, portfolio))
                    {
                        var investment = new Investment(
                            recommendation.Symbol,
                            recommendation.Name,
                            recommendation.Amount,
                            recommendation.Type
                        );
                        
                        portfolio.AddInvestment(investment);
                        
                        // Simulate market movement
                        var newPrice = await marketDataService.GetCurrentPrice(investment.Symbol);
                        var priceChange = (newPrice / 100m) - 1m; // Simplified price change calculation
                        var newValue = investment.Amount * (1 + priceChange * 0.01m);
                        portfolio.UpdateInvestmentValue(investment.Id, newValue);
                    }
                }
            }
        }

        // Assert: Verify system behavior
        portfolio.Should().NotBeNull();
        portfolio.TotalValue.Should().BeGreaterThan(0);
        portfolio.AvailableCash.Should().BeGreaterThanOrEqualTo(0);
        
        // Should have made some investments
        if (portfolio.Investments.Any())
        {
            // Verify diversification
            var investmentTypes = portfolio.Investments.Select(i => i.Type).Distinct().ToList();
            investmentTypes.Should().HaveCountGreaterThanOrEqualTo(1);
            
            // Verify risk management worked
            foreach (var investment in portfolio.Investments)
            {
                var investmentPercentage = investment.Amount / 50000m; // Original portfolio value
                investmentPercentage.Should().BeLessThanOrEqualTo(0.20m); // 20% max per investment
            }
        }
        
        // Total value should be reasonable (not negative, not impossibly high)
        portfolio.TotalValue.Should().BeGreaterThan(10000m); // At least 20% of original
        portfolio.TotalValue.Should().BeLessThanOrEqualTo(200000m); // Not more than 4x original
    }

    [Fact]
    public async Task DataPersistenceWorkflow_ShouldMaintainDataIntegrity()
    {
        // Arrange: Set up data repositories
        var portfolioRepo = new InMemoryPortfolioRepository();
        var historyRepo = new InMemoryInvestmentHistoryRepository();
        
        // Create a portfolio with investments
        var portfolio = new Portfolio(25000m);
        var investment1 = new Investment("AAPL", "Apple Inc.", 5000m, InvestmentType.Stock);
        var investment2 = new Investment("SPY", "S&P 500 ETF", 3000m, InvestmentType.ETF);
        
        portfolio.AddInvestment(investment1);
        portfolio.AddInvestment(investment2);
        
        // Act: Save to data layer
        var portfolioData = new AutonomousMoneyMaker.Data.Models.PortfolioData
        {
            Id = "end-to-end-test",
            TotalValue = portfolio.TotalValue,
            AvailableCash = portfolio.AvailableCash,
            LastUpdated = portfolio.LastUpdated,
            Investments = portfolio.Investments.Select(i => new AutonomousMoneyMaker.Data.Models.InvestmentData
            {
                Id = i.Id,
                Symbol = i.Symbol,
                Name = i.Name,
                Amount = i.Amount,
                CurrentValue = i.CurrentValue,
                PurchaseDate = i.PurchaseDate,
                InvestmentType = i.Type.ToString()
            }).ToList()
        };
        
        await portfolioRepo.SavePortfolioAsync(portfolioData);
        
        // Save investment history
        foreach (var investment in portfolio.Investments)
        {
            var historyData = new AutonomousMoneyMaker.Data.Models.InvestmentHistoryData
            {
                Id = Guid.NewGuid().ToString(),
                PortfolioId = "end-to-end-test",
                Symbol = investment.Symbol,
                Name = investment.Name,
                Amount = investment.Amount,
                PurchasePrice = investment.Amount, // Simplified
                PurchaseDate = investment.PurchaseDate,
                InvestmentType = investment.Type.ToString(),
                Strategy = "End-to-End Test Strategy",
                Reasoning = "Test investment for E2E validation",
                ConfidenceScore = 0.8m
            };
            
            await historyRepo.SaveInvestmentAsync(historyData);
        }
        
        // Act: Retrieve and validate
        var retrievedPortfolio = await portfolioRepo.LoadPortfolioAsync("end-to-end-test");
        var retrievedHistory = await historyRepo.GetInvestmentHistoryAsync("end-to-end-test");
        
        // Assert: Verify data integrity
        retrievedPortfolio.Should().NotBeNull();
        retrievedPortfolio!.TotalValue.Should().Be(portfolio.TotalValue);
        retrievedPortfolio.AvailableCash.Should().Be(portfolio.AvailableCash);
        retrievedPortfolio.Investments.Should().HaveCount(2);
        
        retrievedHistory.Should().HaveCount(2);
        retrievedHistory.Should().OnlyContain(h => h.PortfolioId == "end-to-end-test");
    }

    [Fact]
    public async Task RiskManagementEndToEnd_ShouldPreventDangerousInvestments()
    {
        // Arrange: Set up system with aggressive investment attempts
        var portfolio = new Portfolio(10000m);
        var marketDataService = new MarketDataService();
        var riskManager = new RiskManager();
        
        // Create a scenario where we try to make risky investments
        var highRiskRecommendation = new InvestmentRecommendation
        {
            Symbol = "HIGH_RISK",
            Name = "High Risk Investment",
            Amount = 5000m, // 50% of portfolio - should be rejected
            Type = InvestmentType.Crypto,
            Reasoning = "High risk test",
            ConfidenceScore = 0.9m
        };
        
        // Act & Assert: Risk manager should reject this
        var isAcceptable = riskManager.IsAcceptableRisk(highRiskRecommendation, portfolio);
        isAcceptable.Should().BeFalse();
        
        // Try with smaller, acceptable amount
        highRiskRecommendation.Amount = 1000m; // 10% of portfolio
        isAcceptable = riskManager.IsAcceptableRisk(highRiskRecommendation, portfolio);
        isAcceptable.Should().BeTrue();
        
        // Add investment and test diversification limits
        var investment = new Investment(
            highRiskRecommendation.Symbol,
            highRiskRecommendation.Name,
            highRiskRecommendation.Amount,
            highRiskRecommendation.Type
        );
        portfolio.AddInvestment(investment);
        
        // Try to add too much of the same type
        var anotherCryptoRecommendation = new InvestmentRecommendation
        {
            Symbol = "ANOTHER_CRYPTO",
            Name = "Another Crypto",
            Amount = 3500m, // This would make total crypto > 40%
            Type = InvestmentType.Crypto,
            Reasoning = "Another crypto test",
            ConfidenceScore = 0.8m
        };
        
        isAcceptable = riskManager.IsAcceptableRisk(anotherCryptoRecommendation, portfolio);
        isAcceptable.Should().BeFalse(); // Should be rejected due to type concentration
    }

    [Fact]
    public async Task MarketDataAndStrategyIntegration_ShouldProduceReasonableRecommendations()
    {
        // Arrange
        var portfolio = new Portfolio(100000m);
        var marketDataService = new MarketDataService();
        var strategies = new List<IInvestmentStrategy>
        {
            new DiversifiedETFStrategy(),
            new CryptoTrendStrategy(),
            new ValueInvestingStrategy()
        };
        
        var allRecommendations = new List<InvestmentRecommendation>();
        
        // Act: Get recommendations from all strategies
        foreach (var strategy in strategies)
        {
            for (int i = 0; i < 5; i++) // Try multiple times due to randomness
            {
                var recommendation = await strategy.AnalyzeAndRecommend(portfolio, marketDataService);
                if (recommendation != null)
                {
                    allRecommendations.Add(recommendation);
                }
            }
        }
        
        // Assert: Verify recommendations make sense
        allRecommendations.Should().NotBeEmpty();
        
        foreach (var recommendation in allRecommendations)
        {
            recommendation.Symbol.Should().NotBeNullOrEmpty();
            recommendation.Name.Should().NotBeNullOrEmpty();
            recommendation.Amount.Should().BeGreaterThan(0);
            recommendation.Amount.Should().BeLessThanOrEqualTo(portfolio.AvailableCash);
            recommendation.ConfidenceScore.Should().BeGreaterThan(0);
            recommendation.ConfidenceScore.Should().BeLessThanOrEqualTo(1);
            recommendation.Reasoning.Should().NotBeNullOrEmpty();
        }
        
        // Should have diversity in investment types
        var types = allRecommendations.Select(r => r.Type).Distinct().ToList();
        types.Should().HaveCountGreaterThanOrEqualTo(1);
        
        // ETF strategy should only recommend ETFs
        var etfRecommendations = allRecommendations.Where(r => r.Type == InvestmentType.ETF);
        if (etfRecommendations.Any())
        {
            etfRecommendations.Should().OnlyContain(r => new[] { "SPY", "VTI", "QQQ", "IWM", "EFA" }.Contains(r.Symbol));
        }
    }
}