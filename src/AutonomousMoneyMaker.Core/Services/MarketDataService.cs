namespace AutonomousMoneyMaker.Core.Services;

public class MarketDataService
{
    private readonly Random _random = new();
    private readonly Dictionary<string, decimal> _mockPrices = new();

    public async Task<decimal> GetCurrentPrice(string symbol)
    {
        await Task.Delay(100); // Simulate API call

        if (!_mockPrices.ContainsKey(symbol))
        {
            _mockPrices[symbol] = GenerateRandomPrice();
        }

        // Simulate price fluctuation
        var currentPrice = _mockPrices[symbol];
        var change = (decimal)(_random.NextDouble() * 0.1 - 0.05); // ±5% change
        var newPrice = currentPrice * (1 + change);
        
        _mockPrices[symbol] = Math.Max(0.01m, newPrice); // Ensure price doesn't go below $0.01
        
        return _mockPrices[symbol];
    }

    public async Task<List<string>> GetTrendingSymbols()
    {
        await Task.Delay(200); // Simulate API call
        
        var symbols = new List<string>
        {
            "AAPL", "GOOGL", "MSFT", "TSLA", "AMZN",
            "BTC", "ETH", "SPY", "QQQ", "VTI"
        };
        
        return symbols.OrderBy(_ => _random.Next()).Take(5).ToList();
    }

    private decimal GenerateRandomPrice()
    {
        return (decimal)(_random.NextDouble() * 1000 + 10); // Random price between $10-$1010
    }
}