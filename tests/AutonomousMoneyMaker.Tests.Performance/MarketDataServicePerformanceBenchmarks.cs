using BenchmarkDotNet.Attributes;
using AutonomousMoneyMaker.Core.Services;

namespace AutonomousMoneyMaker.Tests.Performance;

[MemoryDiagnoser]
[SimpleJob]
public class MarketDataServicePerformanceBenchmarks
{
    private MarketDataService _marketDataService = null!;
    private List<string> _symbols = null!;

    [GlobalSetup]
    public void Setup()
    {
        _marketDataService = new MarketDataService();
        _symbols = new List<string>
        {
            "AAPL", "GOOGL", "MSFT", "AMZN", "TSLA", "META", "NVDA", "NFLX", "BRK.B", "JPM",
            "JNJ", "V", "PG", "HD", "UNH", "MA", "DIS", "BAC", "ADBE", "CRM"
        };
    }

    [Benchmark]
    public async Task GetSinglePrice()
    {
        await _marketDataService.GetCurrentPrice("AAPL");
    }

    [Benchmark]
    public async Task Get20PricesSequentially()
    {
        foreach (var symbol in _symbols)
        {
            await _marketDataService.GetCurrentPrice(symbol);
        }
    }

    [Benchmark]
    public async Task Get20PricesInParallel()
    {
        var tasks = _symbols.Select(symbol => _marketDataService.GetCurrentPrice(symbol));
        await Task.WhenAll(tasks);
    }

    [Benchmark]
    public async Task GetTrendingSymbols()
    {
        await _marketDataService.GetTrendingSymbols();
    }

    [Benchmark]
    public async Task GetTrendingSymbols10Times()
    {
        for (int i = 0; i < 10; i++)
        {
            await _marketDataService.GetTrendingSymbols();
        }
    }

    [Benchmark]
    public async Task GetSamePriceMultipleTimes()
    {
        // This tests the price fluctuation performance
        for (int i = 0; i < 100; i++)
        {
            await _marketDataService.GetCurrentPrice("AAPL");
        }
    }
}