using AutonomousMoneyMaker.Core.Models;
using AutonomousMoneyMaker.Core.Services;

namespace AutonomousMoneyMaker.Core.Strategies;

public class DiversifiedETFStrategy : IInvestmentStrategy
{
    public string StrategyName => "Diversified ETF Strategy";
    private readonly Random _random = new();

    public async Task<InvestmentRecommendation?> AnalyzeAndRecommend(Portfolio portfolio, MarketDataService marketData)
    {
        await Task.Yield(); // Make method truly async
        
        var etfs = new[]
        {
            ("SPY", "SPDR S&P 500 ETF"),
            ("VTI", "Vanguard Total Stock Market ETF"),
            ("QQQ", "Invesco QQQ Trust"),
            ("IWM", "iShares Russell 2000 ETF"),
            ("EFA", "iShares MSCI EAFE ETF")
        };

        // Check if we need more ETF diversification
        var etfInvestments = portfolio.Investments.Where(i => i.Type == InvestmentType.ETF).ToList();
        var totalEtfValue = etfInvestments.Sum(i => i.CurrentValue);
        var etfPercentage = portfolio.TotalValue > 0 ? totalEtfValue / portfolio.TotalValue : 0;

        if (etfPercentage < 0.30m && portfolio.CanInvest(200)) // Target 30% in ETFs
        {
            var selectedEtf = etfs[_random.Next(etfs.Length)];
            var amount = Math.Min(portfolio.AvailableCash * 0.15m, 500m); // 15% of available cash, max $500

            return new InvestmentRecommendation
            {
                Symbol = selectedEtf.Item1,
                Name = selectedEtf.Item2,
                Amount = amount,
                Type = InvestmentType.ETF,
                Reasoning = "Building diversified ETF foundation for stable growth",
                ConfidenceScore = 0.8m
            };
        }

        return null;
    }
}