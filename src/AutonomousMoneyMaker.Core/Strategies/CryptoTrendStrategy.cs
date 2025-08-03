using AutonomousMoneyMaker.Core.Models;
using AutonomousMoneyMaker.Core.Services;

namespace AutonomousMoneyMaker.Core.Strategies;

public class CryptoTrendStrategy : IInvestmentStrategy
{
    public string StrategyName => "Crypto Trend Strategy";
    private readonly Random _random = new();

    public async Task<InvestmentRecommendation?> AnalyzeAndRecommend(Portfolio portfolio, MarketDataService marketData)
    {
        await Task.Yield(); // Make method truly async
        
        var cryptos = new[]
        {
            ("BTC", "Bitcoin"),
            ("ETH", "Ethereum"),
            ("ADA", "Cardano"),
            ("SOL", "Solana"),
            ("DOT", "Polkadot")
        };

        // Check crypto allocation - target max 10% in crypto due to volatility
        var cryptoInvestments = portfolio.Investments.Where(i => i.Type == InvestmentType.Crypto).ToList();
        var totalCryptoValue = cryptoInvestments.Sum(i => i.CurrentValue);
        var cryptoPercentage = portfolio.TotalValue > 0 ? totalCryptoValue / portfolio.TotalValue : 0;

        if (cryptoPercentage < 0.10m && portfolio.CanInvest(100))
        {
            var selectedCrypto = cryptos[_random.Next(cryptos.Length)];
            var amount = Math.Min(portfolio.AvailableCash * 0.05m, 300m); // 5% of available cash, max $300

            // Simulate trend analysis (in real implementation, this would use technical indicators)
            var trendScore = _random.NextDouble();
            
            if (trendScore > 0.6) // Only invest if trend looks good
            {
                return new InvestmentRecommendation
                {
                    Symbol = selectedCrypto.Item1,
                    Name = selectedCrypto.Item2,
                    Amount = amount,
                    Type = InvestmentType.Crypto,
                    Reasoning = $"Positive trend detected for {selectedCrypto.Item2}",
                    ConfidenceScore = (decimal)trendScore
                };
            }
        }

        return null;
    }
}