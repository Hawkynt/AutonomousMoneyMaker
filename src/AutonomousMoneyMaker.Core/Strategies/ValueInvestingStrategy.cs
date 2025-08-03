using AutonomousMoneyMaker.Core.Models;
using AutonomousMoneyMaker.Core.Services;

namespace AutonomousMoneyMaker.Core.Strategies;

public class ValueInvestingStrategy : IInvestmentStrategy
{
    public string StrategyName => "Value Investing Strategy";
    private readonly Random _random = new();

    public async Task<InvestmentRecommendation?> AnalyzeAndRecommend(Portfolio portfolio, MarketDataService marketData)
    {
        var valueStocks = new[]
        {
            ("AAPL", "Apple Inc."),
            ("MSFT", "Microsoft Corporation"),
            ("GOOGL", "Alphabet Inc."),
            ("BRK.B", "Berkshire Hathaway"),
            ("JNJ", "Johnson & Johnson"),
            ("V", "Visa Inc."),
            ("PG", "Procter & Gamble"),
            ("JPM", "JPMorgan Chase & Co.")
        };

        // Check stock allocation
        var stockInvestments = portfolio.Investments.Where(i => i.Type == InvestmentType.Stock).ToList();
        var totalStockValue = stockInvestments.Sum(i => i.CurrentValue);
        var stockPercentage = portfolio.TotalValue > 0 ? totalStockValue / portfolio.TotalValue : 0;

        if (stockPercentage < 0.40m && portfolio.CanInvest(150)) // Target up to 40% in individual stocks
        {
            var selectedStock = valueStocks[_random.Next(valueStocks.Length)];
            
            // Simulate value analysis (in real implementation, this would analyze P/E ratios, etc.)
            var currentPrice = await marketData.GetCurrentPrice(selectedStock.Item1);
            var valueScore = _random.NextDouble();
            
            if (valueScore > 0.7) // Only invest in undervalued stocks
            {
                var amount = Math.Min(portfolio.AvailableCash * 0.10m, 600m); // 10% of available cash, max $600

                return new InvestmentRecommendation
                {
                    Symbol = selectedStock.Item1,
                    Name = selectedStock.Item2,
                    Amount = amount,
                    Type = InvestmentType.Stock,
                    Reasoning = $"Value opportunity detected in {selectedStock.Item2}",
                    ConfidenceScore = (decimal)valueScore
                };
            }
        }

        return null;
    }
}