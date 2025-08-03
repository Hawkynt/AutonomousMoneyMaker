using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using AutonomousMoneyMaker.Core.Models;

namespace AutonomousMoneyMaker.Tests.Performance;

[MemoryDiagnoser]
[SimpleJob]
public class PortfolioPerformanceBenchmarks
{
    private Portfolio _portfolio = null!;
    private List<Investment> _investments = null!;

    [GlobalSetup]
    public void Setup()
    {
        _portfolio = new Portfolio(100000m);
        _investments = new List<Investment>();
        
        // Pre-create investments for testing
        for (int i = 0; i < 1000; i++)
        {
            _investments.Add(new Investment($"STOCK{i}", $"Stock {i}", 100m, InvestmentType.Stock));
        }
    }

    [Benchmark]
    public void AddSingleInvestment()
    {
        var portfolio = new Portfolio(10000m);
        var investment = new Investment("AAPL", "Apple Inc.", 1000m, InvestmentType.Stock);
        portfolio.AddInvestment(investment);
    }

    [Benchmark]
    public void Add100Investments()
    {
        var portfolio = new Portfolio(100000m);
        for (int i = 0; i < 100; i++)
        {
            var investment = new Investment($"STOCK{i}", $"Stock {i}", 100m, InvestmentType.Stock);
            portfolio.AddInvestment(investment);
        }
    }

    [Benchmark]
    public void CalculateTotalProfitWith1000Investments()
    {
        var portfolio = new Portfolio(1000000m);
        
        // Add investments
        foreach (var investment in _investments)
        {
            var newInvestment = new Investment(investment.Symbol, investment.Name, investment.Amount, investment.Type);
            portfolio.AddInvestment(newInvestment);
            
            // Simulate some profit/loss
            portfolio.UpdateInvestmentValue(newInvestment.Id, investment.Amount * 1.1m);
        }

        // Benchmark the profit calculation
        portfolio.GetTotalProfit();
    }

    [Benchmark]
    public void UpdateInvestmentValues1000Times()
    {
        var portfolio = new Portfolio(1000000m);
        var investments = new List<Investment>();
        
        // Add investments
        for (int i = 0; i < 1000; i++)
        {
            var investment = new Investment($"STOCK{i}", $"Stock {i}", 100m, InvestmentType.Stock);
            portfolio.AddInvestment(investment);
            investments.Add(investment);
        }

        // Benchmark updating all investment values
        foreach (var investment in investments)
        {
            portfolio.UpdateInvestmentValue(investment.Id, investment.Amount * 1.05m);
        }
    }

    [Benchmark]
    public void CheckCanInvest1000Times()
    {
        var portfolio = new Portfolio(100000m);
        
        for (int i = 0; i < 1000; i++)
        {
            portfolio.CanInvest(100m);
        }
    }
}

