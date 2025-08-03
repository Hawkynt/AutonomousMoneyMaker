using AutonomousMoneyMaker.Core.Models;
using FluentAssertions;

namespace AutonomousMoneyMaker.Tests.Unit.Models;

public class InvestmentTests
{
    [Fact]
    public void Constructor_ShouldInitializeCorrectly()
    {
        var symbol = "AAPL";
        var name = "Apple Inc.";
        var amount = 1000m;
        var type = InvestmentType.Stock;

        var investment = new Investment(symbol, name, amount, type);

        investment.Id.Should().NotBeNullOrEmpty();
        investment.Symbol.Should().Be(symbol);
        investment.Name.Should().Be(name);
        investment.Amount.Should().Be(amount);
        investment.CurrentValue.Should().Be(amount); // Initially equal to amount
        investment.Type.Should().Be(type);
        investment.PurchaseDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void GetProfitLoss_WithProfit_ShouldReturnPositiveValue()
    {
        var investment = new Investment("AAPL", "Apple Inc.", 1000m, InvestmentType.Stock);
        investment.CurrentValue = 1200m;

        investment.GetProfitLoss().Should().Be(200m);
    }

    [Fact]
    public void GetProfitLoss_WithLoss_ShouldReturnNegativeValue()
    {
        var investment = new Investment("AAPL", "Apple Inc.", 1000m, InvestmentType.Stock);
        investment.CurrentValue = 800m;

        investment.GetProfitLoss().Should().Be(-200m);
    }

    [Fact]
    public void GetProfitLoss_WithNoChange_ShouldReturnZero()
    {
        var investment = new Investment("AAPL", "Apple Inc.", 1000m, InvestmentType.Stock);
        
        investment.GetProfitLoss().Should().Be(0m);
    }

    [Fact]
    public void GetProfitLossPercentage_WithProfit_ShouldReturnCorrectPercentage()
    {
        var investment = new Investment("AAPL", "Apple Inc.", 1000m, InvestmentType.Stock);
        investment.CurrentValue = 1200m;

        investment.GetProfitLossPercentage().Should().Be(20m);
    }

    [Fact]
    public void GetProfitLossPercentage_WithLoss_ShouldReturnNegativePercentage()
    {
        var investment = new Investment("AAPL", "Apple Inc.", 1000m, InvestmentType.Stock);
        investment.CurrentValue = 800m;

        investment.GetProfitLossPercentage().Should().Be(-20m);
    }

    [Fact]
    public void GetProfitLossPercentage_WithZeroAmount_ShouldReturnZero()
    {
        var investment = new Investment("AAPL", "Apple Inc.", 0m, InvestmentType.Stock);
        investment.CurrentValue = 100m;

        investment.GetProfitLossPercentage().Should().Be(0m);
    }

    [Fact]
    public void InvestmentType_AllEnumValues_ShouldBeAvailable()
    {
        var expectedTypes = new[]
        {
            InvestmentType.Stock,
            InvestmentType.Crypto,
            InvestmentType.Bond,
            InvestmentType.ETF,
            InvestmentType.Commodity,
            InvestmentType.RealEstate
        };

        foreach (var type in expectedTypes)
        {
            var investment = new Investment("TEST", "Test", 100m, type);
            investment.Type.Should().Be(type);
        }
    }
}