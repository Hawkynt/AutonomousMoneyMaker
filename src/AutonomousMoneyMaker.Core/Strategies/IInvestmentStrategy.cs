using AutonomousMoneyMaker.Core.Models;
using AutonomousMoneyMaker.Core.Services;

namespace AutonomousMoneyMaker.Core.Strategies;

public interface IInvestmentStrategy
{
    Task<InvestmentRecommendation?> AnalyzeAndRecommend(Portfolio portfolio, MarketDataService marketData);
    string StrategyName { get; }
}

public class InvestmentRecommendation
{
    public string Symbol { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public InvestmentType Type { get; set; }
    public string Reasoning { get; set; } = string.Empty;
    public decimal ConfidenceScore { get; set; } // 0-1 scale
}