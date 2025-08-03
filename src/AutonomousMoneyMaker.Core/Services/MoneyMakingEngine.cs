using AutonomousMoneyMaker.Core.Models;
using AutonomousMoneyMaker.Core.Strategies;

namespace AutonomousMoneyMaker.Core.Services;

public class MoneyMakingEngine
{
    private readonly List<IInvestmentStrategy> _strategies;
    private readonly MarketDataService _marketDataService;
    private readonly RiskManager _riskManager;

    public MoneyMakingEngine()
    {
        _strategies = new List<IInvestmentStrategy>
        {
            new DiversifiedETFStrategy(),
            new CryptoTrendStrategy(),
            new ValueInvestingStrategy()
        };
        _marketDataService = new MarketDataService();
        _riskManager = new RiskManager();
    }

    public async Task StartAsync(Portfolio portfolio)
    {
        Console.WriteLine($"💼 Starting with portfolio value: ${portfolio.TotalValue:F2}");
        Console.WriteLine($"💵 Available cash: ${portfolio.AvailableCash:F2}");
        
        while (true)
        {
            try
            {
                await AnalyzeAndInvest(portfolio);
                await UpdatePortfolioValues(portfolio);
                DisplayPortfolioStatus(portfolio);
                
                await Task.Delay(TimeSpan.FromMinutes(1)); // Check every minute
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error in money making process: {ex.Message}");
                await Task.Delay(TimeSpan.FromMinutes(5)); // Wait longer on error
            }
        }
    }

    private async Task AnalyzeAndInvest(Portfolio portfolio)
    {
        foreach (var strategy in _strategies)
        {
            if (portfolio.CanInvest(100)) // Minimum investment of $100
            {
                var recommendation = await strategy.AnalyzeAndRecommend(portfolio, _marketDataService);
                
                if (recommendation != null && _riskManager.IsAcceptableRisk(recommendation, portfolio))
                {
                    var investment = new Investment(
                        recommendation.Symbol,
                        recommendation.Name,
                        recommendation.Amount,
                        recommendation.Type
                    );
                    
                    portfolio.AddInvestment(investment);
                    Console.WriteLine($"📈 New investment: {investment.Symbol} - ${investment.Amount:F2}");
                }
            }
        }
    }

    private async Task UpdatePortfolioValues(Portfolio portfolio)
    {
        foreach (var investment in portfolio.Investments)
        {
            var currentPrice = await _marketDataService.GetCurrentPrice(investment.Symbol);
            if (currentPrice > 0)
            {
                var newValue = investment.Amount * (currentPrice / 100); // Simplified calculation
                portfolio.UpdateInvestmentValue(investment.Id, newValue);
            }
        }
    }

    private void DisplayPortfolioStatus(Portfolio portfolio)
    {
        Console.WriteLine($"\n📊 Portfolio Status:");
        Console.WriteLine($"   Total Value: ${portfolio.TotalValue:F2}");
        Console.WriteLine($"   Available Cash: ${portfolio.AvailableCash:F2}");
        Console.WriteLine($"   Total Profit: ${portfolio.GetTotalProfit():F2}");
        Console.WriteLine($"   Investments: {portfolio.Investments.Count}");
        Console.WriteLine($"   Last Updated: {portfolio.LastUpdated:HH:mm:ss}\n");
    }
}