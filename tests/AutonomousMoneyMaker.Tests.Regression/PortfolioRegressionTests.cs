using AutonomousMoneyMaker.Core.Models;
using FluentAssertions;

namespace AutonomousMoneyMaker.Tests.Regression;

/// <summary>
/// Regression tests to ensure that previously working functionality continues to work
/// These tests capture the behavior as of the implementation date and should not change
/// unless the behavior intentionally changes
/// </summary>
public class PortfolioRegressionTests
{
    [Fact]
    public void Portfolio_DefaultConstructor_ShouldAlwaysInitializeWith1000Dollars()
    {
        // This behavior should never change for backward compatibility
        var portfolio = new Portfolio();
        
        portfolio.TotalValue.Should().Be(1000m);
        portfolio.AvailableCash.Should().Be(1000m);
        portfolio.Investments.Should().BeEmpty();
    }

    [Fact]
    public void Portfolio_GetTotalProfit_WithZeroInvestments_ShouldAlwaysReturnZero()
    {
        // This is a regression test to ensure this behavior doesn't accidentally change
        var portfolio = new Portfolio(5000m);
        
        portfolio.GetTotalProfit().Should().Be(0m);
    }

    [Fact]
    public void Investment_ProfitLossCalculation_ShouldMaintainPrecision()
    {
        // Regression test for specific precision requirements
        var investment = new Investment("TEST", "Test Stock", 1000.123m, InvestmentType.Stock);
        investment.CurrentValue = 1100.456m;
        
        investment.GetProfitLoss().Should().Be(100.333m);
        investment.GetProfitLossPercentage().Should().BeApproximately(10.03307m, 0.00001m);
    }

    [Fact]
    public void Portfolio_AddInvestment_ShouldMaintainCashAccuracy()
    {
        // Regression test for cash calculation accuracy
        var portfolio = new Portfolio(10000.50m);
        var investment = new Investment("TEST", "Test", 3333.33m, InvestmentType.Stock);
        
        portfolio.AddInvestment(investment);
        
        portfolio.AvailableCash.Should().Be(6667.17m);
    }

    [Fact]
    public void Investment_Id_ShouldAlwaysBeUnique()
    {
        // Regression test to ensure IDs remain unique
        var investments = new List<Investment>();
        
        for (int i = 0; i < 1000; i++)
        {
            investments.Add(new Investment("TEST", "Test", 100m, InvestmentType.Stock));
        }
        
        var uniqueIds = investments.Select(i => i.Id).Distinct().ToList();
        uniqueIds.Should().HaveCount(1000);
    }

    [Fact]
    public void Portfolio_UpdateNonExistentInvestment_ShouldNotAffectPortfolio()
    {
        // Regression test for specific error handling behavior
        var portfolio = new Portfolio(10000m);
        var investment = new Investment("AAPL", "Apple", 1000m, InvestmentType.Stock);
        portfolio.AddInvestment(investment);
        
        var originalTotalValue = portfolio.TotalValue;
        var originalCash = portfolio.AvailableCash;
        
        portfolio.UpdateInvestmentValue("non-existent-id", 5000m);
        
        portfolio.TotalValue.Should().Be(originalTotalValue);
        portfolio.AvailableCash.Should().Be(originalCash);
        investment.CurrentValue.Should().Be(1000m); // Should remain unchanged
    }

    [Fact]
    public void InvestmentType_EnumValues_ShouldRemainStable()
    {
        // Regression test to ensure enum values don't change (important for serialization)
        var expectedTypes = new[]
        {
            InvestmentType.Stock,
            InvestmentType.Crypto,
            InvestmentType.Bond,
            InvestmentType.ETF,
            InvestmentType.Commodity,
            InvestmentType.RealEstate
        };
        
        var actualTypes = Enum.GetValues<InvestmentType>();
        actualTypes.Should().BeEquivalentTo(expectedTypes);
    }

    [Fact]
    public void Investment_PurchaseDate_ShouldBeSetToCurrentTime()
    {
        // Regression test for purchase date behavior
        var beforeCreation = DateTime.UtcNow;
        var investment = new Investment("TEST", "Test", 100m, InvestmentType.Stock);
        var afterCreation = DateTime.UtcNow;
        
        investment.PurchaseDate.Should().BeAfter(beforeCreation.AddSeconds(-1));
        investment.PurchaseDate.Should().BeBefore(afterCreation.AddSeconds(1));
    }
}