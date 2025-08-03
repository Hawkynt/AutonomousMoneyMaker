using AutonomousMoneyMaker.Core.Models;
using FluentAssertions;

namespace AutonomousMoneyMaker.Tests.Unit.Models;

public class PortfolioTests
{
    [Fact]
    public void Constructor_WithDefaultValues_ShouldInitializeCorrectly()
    {
        var portfolio = new Portfolio();

        portfolio.TotalValue.Should().Be(1000m);
        portfolio.AvailableCash.Should().Be(1000m);
        portfolio.Investments.Should().BeEmpty();
        portfolio.LastUpdated.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void Constructor_WithCustomCash_ShouldInitializeCorrectly()
    {
        var initialCash = 5000m;
        var portfolio = new Portfolio(initialCash);

        portfolio.TotalValue.Should().Be(initialCash);
        portfolio.AvailableCash.Should().Be(initialCash);
        portfolio.Investments.Should().BeEmpty();
    }

    [Fact]
    public void CanInvest_WithSufficientFunds_ShouldReturnTrue()
    {
        var portfolio = new Portfolio(1000m);

        portfolio.CanInvest(500m).Should().BeTrue();
    }

    [Fact]
    public void CanInvest_WithInsufficientFunds_ShouldReturnFalse()
    {
        var portfolio = new Portfolio(1000m);

        portfolio.CanInvest(1500m).Should().BeFalse();
    }

    [Fact]
    public void AddInvestment_WithSufficientFunds_ShouldAddInvestmentAndUpdateCash()
    {
        var portfolio = new Portfolio(1000m);
        var investment = new Investment("AAPL", "Apple Inc.", 500m, InvestmentType.Stock);

        portfolio.AddInvestment(investment);

        portfolio.AvailableCash.Should().Be(500m);
        portfolio.TotalValue.Should().Be(1000m); // Still 1000 because investment current value = amount
        portfolio.Investments.Should().HaveCount(1);
        portfolio.Investments.Should().Contain(investment);
    }

    [Fact]
    public void AddInvestment_WithInsufficientFunds_ShouldThrowException()
    {
        var portfolio = new Portfolio(1000m);
        var investment = new Investment("AAPL", "Apple Inc.", 1500m, InvestmentType.Stock);

        var action = () => portfolio.AddInvestment(investment);

        action.Should().Throw<InvalidOperationException>()
            .WithMessage("Insufficient funds for investment");
    }

    [Fact]
    public void UpdateInvestmentValue_WithExistingInvestment_ShouldUpdateValue()
    {
        var portfolio = new Portfolio(1000m);
        var investment = new Investment("AAPL", "Apple Inc.", 500m, InvestmentType.Stock);
        portfolio.AddInvestment(investment);

        portfolio.UpdateInvestmentValue(investment.Id, 600m);

        portfolio.TotalValue.Should().Be(1100m); // 500 cash + 600 investment value
        investment.CurrentValue.Should().Be(600m);
    }

    [Fact]
    public void UpdateInvestmentValue_WithNonExistentInvestment_ShouldNotChangeAnything()
    {
        var portfolio = new Portfolio(1000m);
        var investment = new Investment("AAPL", "Apple Inc.", 500m, InvestmentType.Stock);
        portfolio.AddInvestment(investment);
        var originalTotalValue = portfolio.TotalValue;

        portfolio.UpdateInvestmentValue("non-existent-id", 600m);

        portfolio.TotalValue.Should().Be(originalTotalValue);
        investment.CurrentValue.Should().Be(500m);
    }

    [Fact]
    public void GetTotalProfit_WithProfitableInvestments_ShouldReturnCorrectProfit()
    {
        var portfolio = new Portfolio(1000m);
        var investment1 = new Investment("AAPL", "Apple Inc.", 500m, InvestmentType.Stock);
        var investment2 = new Investment("GOOGL", "Alphabet Inc.", 300m, InvestmentType.Stock);
        
        portfolio.AddInvestment(investment1);
        portfolio.AddInvestment(investment2);
        
        portfolio.UpdateInvestmentValue(investment1.Id, 600m); // +100 profit
        portfolio.UpdateInvestmentValue(investment2.Id, 250m); // -50 loss

        portfolio.GetTotalProfit().Should().Be(50m); // 100 - 50
    }

    [Fact]
    public void GetTotalProfit_WithNoInvestments_ShouldReturnZero()
    {
        var portfolio = new Portfolio(1000m);

        portfolio.GetTotalProfit().Should().Be(0m);
    }
}