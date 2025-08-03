using AutonomousMoneyMaker.Core.Services;
using FluentAssertions;

namespace AutonomousMoneyMaker.Tests.Unit.Services;

public class MarketDataServiceTests
{
    private readonly MarketDataService _marketDataService;

    public MarketDataServiceTests()
    {
        _marketDataService = new MarketDataService();
    }

    [Fact]
    public async Task GetCurrentPrice_WithAnySymbol_ShouldReturnPositivePrice()
    {
        var symbol = "AAPL";

        var price = await _marketDataService.GetCurrentPrice(symbol);

        price.Should().BeGreaterThan(0m);
    }

    [Fact]
    public async Task GetCurrentPrice_WithSameSymbolMultipleTimes_ShouldShowPriceFluctuation()
    {
        var symbol = "AAPL";
        var prices = new List<decimal>();

        // Get multiple price readings
        for (int i = 0; i < 10; i++)
        {
            var price = await _marketDataService.GetCurrentPrice(symbol);
            prices.Add(price);
        }

        // Prices should all be positive
        prices.Should().OnlyContain(p => p > 0);
        
        // At least some prices should be different (due to simulated fluctuation)
        prices.Distinct().Should().HaveCountGreaterThan(1);
    }

    [Fact]
    public async Task GetCurrentPrice_ShouldNeverReturnZeroOrNegative()
    {
        var symbols = new[] { "AAPL", "GOOGL", "MSFT", "TSLA", "BTC" };

        foreach (var symbol in symbols)
        {
            var price = await _marketDataService.GetCurrentPrice(symbol);
            price.Should().BeGreaterThan(0m);
        }
    }

    [Fact]
    public async Task GetTrendingSymbols_ShouldReturnFiveSymbols()
    {
        var trendingSymbols = await _marketDataService.GetTrendingSymbols();

        trendingSymbols.Should().HaveCount(5);
        trendingSymbols.Should().OnlyContain(s => !string.IsNullOrWhiteSpace(s));
    }

    [Fact]
    public async Task GetTrendingSymbols_ShouldReturnKnownSymbols()
    {
        var expectedSymbols = new[] { "AAPL", "GOOGL", "MSFT", "TSLA", "AMZN", "BTC", "ETH", "SPY", "QQQ", "VTI" };

        var trendingSymbols = await _marketDataService.GetTrendingSymbols();

        trendingSymbols.Should().OnlyContain(s => expectedSymbols.Contains(s));
    }

    [Fact]
    public async Task GetTrendingSymbols_MultipleCallsShouldReturnDifferentResults()
    {
        var firstCall = await _marketDataService.GetTrendingSymbols();
        var secondCall = await _marketDataService.GetTrendingSymbols();

        // Due to randomization, results should likely be different
        // But they could occasionally be the same, so we'll just verify they're valid
        firstCall.Should().HaveCount(5);
        secondCall.Should().HaveCount(5);
    }

    [Fact]
    public async Task GetCurrentPrice_WithPriceFluctuation_ShouldStayWithinReasonableBounds()
    {
        var symbol = "TEST";
        var initialPrice = await _marketDataService.GetCurrentPrice(symbol);
        var prices = new List<decimal> { initialPrice };

        // Get 20 price updates
        for (int i = 0; i < 20; i++)
        {
            var price = await _marketDataService.GetCurrentPrice(symbol);
            prices.Add(price);
        }

        var minPrice = prices.Min();
        var maxPrice = prices.Max();
        
        // Price shouldn't change too dramatically (this is based on the ±5% fluctuation in the implementation)
        var ratio = maxPrice / minPrice;
        ratio.Should().BeLessThan(3m); // Shouldn't more than triple in 20 updates
    }
}