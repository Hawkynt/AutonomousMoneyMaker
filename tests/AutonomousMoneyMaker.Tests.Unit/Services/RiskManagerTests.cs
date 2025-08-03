using AutonomousMoneyMaker.Core.Models;
using AutonomousMoneyMaker.Core.Services;
using AutonomousMoneyMaker.Core.Strategies;
using FluentAssertions;

namespace AutonomousMoneyMaker.Tests.Unit.Services;

public class RiskManagerTests
{
    private readonly RiskManager _riskManager;

    public RiskManagerTests()
    {
        _riskManager = new RiskManager();
    }

    [Fact]
    public void IsAcceptableRisk_WithNormalInvestment_ShouldReturnTrue()
    {
        var portfolio = new Portfolio(10000m);
        var recommendation = new InvestmentRecommendation
        {
            Symbol = "AAPL",
            Amount = 1000m, // 10% of portfolio
            Type = InvestmentType.Stock
        };

        var result = _riskManager.IsAcceptableRisk(recommendation, portfolio);

        result.Should().BeTrue();
    }

    [Fact]
    public void IsAcceptableRisk_WithTooLargeInvestment_ShouldReturnFalse()
    {
        var portfolio = new Portfolio(10000m);
        var recommendation = new InvestmentRecommendation
        {
            Symbol = "AAPL",
            Amount = 2500m, // 25% of portfolio (exceeds 20% limit)
            Type = InvestmentType.Stock
        };

        var result = _riskManager.IsAcceptableRisk(recommendation, portfolio);

        result.Should().BeFalse();
    }

    [Fact]
    public void IsAcceptableRisk_WithTooMuchOfSameType_ShouldReturnFalse()
    {
        var portfolio = new Portfolio(10000m);
        
        // Add existing crypto investments totaling 3000
        var existingCrypto1 = new Investment("BTC", "Bitcoin", 1500m, InvestmentType.Crypto);
        var existingCrypto2 = new Investment("ETH", "Ethereum", 1500m, InvestmentType.Crypto);
        portfolio.AddInvestment(existingCrypto1);
        portfolio.AddInvestment(existingCrypto2);

        // Try to add more crypto that would exceed 40% limit
        var recommendation = new InvestmentRecommendation
        {
            Symbol = "ADA",
            Amount = 1500m, // Would make total crypto 4500/10000 = 45%
            Type = InvestmentType.Crypto
        };

        var result = _riskManager.IsAcceptableRisk(recommendation, portfolio);

        result.Should().BeFalse();
    }

    [Fact]
    public void IsAcceptableRisk_WithTooManyOfSameSymbol_ShouldReturnFalse()
    {
        var portfolio = new Portfolio(10000m);
        
        // Add 3 AAPL investments
        for (int i = 0; i < 3; i++)
        {
            var investment = new Investment("AAPL", "Apple Inc.", 500m, InvestmentType.Stock);
            portfolio.AddInvestment(investment);
        }

        // Try to add 4th AAPL investment
        var recommendation = new InvestmentRecommendation
        {
            Symbol = "AAPL",
            Amount = 500m,
            Type = InvestmentType.Stock
        };

        var result = _riskManager.IsAcceptableRisk(recommendation, portfolio);

        result.Should().BeFalse();
    }

    [Fact]
    public void CalculateRiskScore_WithEmptyPortfolio_ShouldReturnZero()
    {
        var portfolio = new Portfolio(10000m);

        var riskScore = _riskManager.CalculateRiskScore(portfolio);

        riskScore.Should().Be(0m);
    }

    [Fact]
    public void CalculateRiskScore_WithHighCryptoAllocation_ShouldReturnHighRisk()
    {
        var portfolio = new Portfolio(10000m);
        
        // Add significant crypto allocation
        var cryptoInvestment = new Investment("BTC", "Bitcoin", 3000m, InvestmentType.Crypto);
        portfolio.AddInvestment(cryptoInvestment);

        var riskScore = _riskManager.CalculateRiskScore(portfolio);

        riskScore.Should().BeGreaterThan(0m);
    }

    [Fact]
    public void CalculateRiskScore_WithDiversifiedPortfolio_ShouldReturnLowerRisk()
    {
        var portfolio = new Portfolio(10000m);
        
        // Add diversified investments
        portfolio.AddInvestment(new Investment("AAPL", "Apple", 1500m, InvestmentType.Stock));
        portfolio.AddInvestment(new Investment("SPY", "S&P 500 ETF", 1500m, InvestmentType.ETF));
        portfolio.AddInvestment(new Investment("BTC", "Bitcoin", 500m, InvestmentType.Crypto));
        portfolio.AddInvestment(new Investment("BOND", "Treasury Bond", 1000m, InvestmentType.Bond));

        var riskScore = _riskManager.CalculateRiskScore(portfolio);

        // Should have lower risk due to diversification
        riskScore.Should().BeLessThan(5m);
    }
}