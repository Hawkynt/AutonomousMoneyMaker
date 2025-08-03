using AutonomousMoneyMaker.Data.Models;
using System.Collections.Concurrent;

namespace AutonomousMoneyMaker.Data.Repositories;

public class InMemoryPortfolioRepository : IPortfolioRepository
{
    private readonly ConcurrentDictionary<string, PortfolioData> _portfolios = new();

    public Task SavePortfolioAsync(PortfolioData portfolio)
    {
        _portfolios.AddOrUpdate(portfolio.Id, portfolio, (key, existing) => portfolio);
        return Task.CompletedTask;
    }

    public Task<PortfolioData?> LoadPortfolioAsync(string portfolioId)
    {
        _portfolios.TryGetValue(portfolioId, out var portfolio);
        return Task.FromResult(portfolio);
    }

    public Task<List<PortfolioData>> GetAllPortfoliosAsync()
    {
        return Task.FromResult(_portfolios.Values.ToList());
    }

    public Task DeletePortfolioAsync(string portfolioId)
    {
        _portfolios.TryRemove(portfolioId, out _);
        return Task.CompletedTask;
    }
}