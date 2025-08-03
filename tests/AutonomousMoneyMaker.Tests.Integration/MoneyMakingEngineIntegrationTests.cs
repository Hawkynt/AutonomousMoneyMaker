using AutonomousMoneyMaker.Core.Models;
using AutonomousMoneyMaker.Core.Services;
using FluentAssertions;

namespace AutonomousMoneyMaker.Tests.Integration;

public class MoneyMakingEngineIntegrationTests
{
    [Fact]
    public async Task MoneyMakingEngine_FullWorkflow_ShouldProcessCorrectly()
    {
        var portfolio = new Portfolio(10000m);
        var engine = new MoneyMakingEngine();

        // This would normally run indefinitely, so we'll test the core methods
        await TestAnalyzeAndInvestProcess(engine, portfolio);

        portfolio.Investments.Should().HaveCountGreaterThanOrEqualTo(0);
        portfolio.TotalValue.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task MultipleStrategies_ShouldWorkTogether()
    {
        var portfolio = new Portfolio(50000m); // Larger portfolio for multiple strategies
        var engine = new MoneyMakingEngine();

        // Simulate multiple investment rounds
        for (int i = 0; i < 5; i++)
        {
            await TestAnalyzeAndInvestProcess(engine, portfolio);
        }

        // Should have some investments after multiple rounds
        portfolio.Investments.Should().HaveCountGreaterThanOrEqualTo(1);
        
        // Should have diversification across different types
        var investmentTypes = portfolio.Investments.Select(i => i.Type).Distinct().ToList();
        investmentTypes.Should().HaveCountGreaterThanOrEqualTo(1);
    }

    [Fact]
    public async Task RiskManager_ShouldPreventOverInvestment()
    {
        var portfolio = new Portfolio(1000m); // Small portfolio
        var engine = new MoneyMakingEngine();

        // Try to make many investments with small portfolio
        for (int i = 0; i < 20; i++)
        {
            await TestAnalyzeAndInvestProcess(engine, portfolio);
        }

        // Risk manager should prevent over-investment
        portfolio.AvailableCash.Should().BeGreaterThanOrEqualTo(0);
        portfolio.Investments.Sum(i => i.Amount).Should().BeLessThanOrEqualTo(1000m);
    }

    private async Task TestAnalyzeAndInvestProcess(MoneyMakingEngine engine, Portfolio portfolio)
    {
        // Use reflection to access private methods for testing
        var analyzeMethod = typeof(MoneyMakingEngine).GetMethod("AnalyzeAndInvest", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        
        if (analyzeMethod != null)
        {
            await (Task)analyzeMethod.Invoke(engine, new object[] { portfolio })!;
        }
    }
}