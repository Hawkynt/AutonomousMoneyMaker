using AutonomousMoneyMaker.Core.Models;
using AutonomousMoneyMaker.Core.Services;
using AutonomousMoneyMaker.Core.Strategies;
using FluentAssertions;
using Moq;

namespace AutonomousMoneyMaker.Tests.Unit.Strategies;

public class DiversifiedETFStrategyTests
{
    private readonly DiversifiedETFStrategy _strategy;
    private readonly Mock<MarketDataService> _mockMarketDataService;

    public DiversifiedETFStrategyTests()
    {
        _strategy = new DiversifiedETFStrategy();
        _mockMarketDataService = new Mock<MarketDataService>();
    }

    [Fact]
    public void StrategyName_ShouldReturnCorrectName()
    {
        _strategy.StrategyName.Should().Be("Diversified ETF Strategy");
    }

    [Fact]
    public async Task AnalyzeAndRecommend_WithLowETFAllocation_ShouldRecommendETF()
    {
        var portfolio = new Portfolio(10000m);

        var recommendation = await _strategy.AnalyzeAndRecommend(portfolio, _mockMarketDataService.Object);

        recommendation.Should().NotBeNull();
        recommendation!.Type.Should().Be(InvestmentType.ETF);
        recommendation.Amount.Should().BeGreaterThan(0);
        recommendation.Amount.Should().BeLessThanOrEqualTo(500m); // Max amount
        recommendation.Symbol.Should().BeOneOf("SPY", "VTI", "QQQ", "IWM", "EFA");
        recommendation.ConfidenceScore.Should().Be(0.8m);
        recommendation.Reasoning.Should().Be("Building diversified ETF foundation for stable growth");
    }

    [Fact]
    public async Task AnalyzeAndRecommend_WithHighETFAllocation_ShouldNotRecommend()
    {
        var portfolio = new Portfolio(10000m);
        
        // Add ETF investments totaling 35% (above 30% target)
        portfolio.AddInvestment(new Investment("SPY", "S&P 500 ETF", 2000m, InvestmentType.ETF));
        portfolio.AddInvestment(new Investment("VTI", "Total Stock Market ETF", 1500m, InvestmentType.ETF));

        var recommendation = await _strategy.AnalyzeAndRecommend(portfolio, _mockMarketDataService.Object);

        recommendation.Should().BeNull();
    }

    [Fact]
    public async Task AnalyzeAndRecommend_WithInsufficientCash_ShouldNotRecommend()
    {
        var portfolio = new Portfolio(100m); // Only $100 available, below $200 minimum

        var recommendation = await _strategy.AnalyzeAndRecommend(portfolio, _mockMarketDataService.Object);

        recommendation.Should().BeNull();
    }

    [Fact]
    public async Task AnalyzeAndRecommend_ShouldRecommendValidETFSymbols()
    {
        var portfolio = new Portfolio(10000m);
        var validETFs = new[] { "SPY", "VTI", "QQQ", "IWM", "EFA" };

        // Test multiple times to account for randomness
        for (int i = 0; i < 10; i++)
        {
            var recommendation = await _strategy.AnalyzeAndRecommend(portfolio, _mockMarketDataService.Object);
            
            if (recommendation != null)
            {
                validETFs.Should().Contain(recommendation.Symbol);
            }
        }
    }

    [Fact]
    public async Task AnalyzeAndRecommend_AmountShouldBeWithinLimits()
    {
        var portfolio = new Portfolio(10000m);

        var recommendation = await _strategy.AnalyzeAndRecommend(portfolio, _mockMarketDataService.Object);

        recommendation.Should().NotBeNull();
        recommendation!.Amount.Should().BeGreaterThan(0);
        recommendation.Amount.Should().BeLessThanOrEqualTo(500m); // Max amount limit
        recommendation.Amount.Should().BeLessThanOrEqualTo(portfolio.AvailableCash * 0.15m); // 15% of available cash
    }

    [Fact]
    public async Task AnalyzeAndRecommend_WithExactly30PercentETF_ShouldNotRecommend()
    {
        var portfolio = new Portfolio(10000m);
        
        // Add exactly 30% ETF allocation
        portfolio.AddInvestment(new Investment("SPY", "S&P 500 ETF", 3000m, InvestmentType.ETF));

        var recommendation = await _strategy.AnalyzeAndRecommend(portfolio, _mockMarketDataService.Object);

        recommendation.Should().BeNull();
    }
}