using AutonomousMoneyMaker.Data.Models;
using System.Collections.Concurrent;

namespace AutonomousMoneyMaker.Data.Repositories;

public class InMemoryInvestmentHistoryRepository : IInvestmentHistoryRepository
{
    private readonly ConcurrentDictionary<string, List<InvestmentHistoryData>> _investmentHistory = new();

    public Task SaveInvestmentAsync(InvestmentHistoryData investment)
    {
        _investmentHistory.AddOrUpdate(
            investment.PortfolioId,
            new List<InvestmentHistoryData> { investment },
            (key, existing) => 
            {
                existing.Add(investment);
                return existing;
            });
        
        return Task.CompletedTask;
    }

    public Task<List<InvestmentHistoryData>> GetInvestmentHistoryAsync(string portfolioId)
    {
        _investmentHistory.TryGetValue(portfolioId, out var history);
        return Task.FromResult(history ?? new List<InvestmentHistoryData>());
    }

    public Task<List<InvestmentHistoryData>> GetInvestmentsBySymbolAsync(string symbol)
    {
        var investments = _investmentHistory.Values
            .SelectMany(list => list)
            .Where(i => i.Symbol.Equals(symbol, StringComparison.OrdinalIgnoreCase))
            .ToList();
            
        return Task.FromResult(investments);
    }
}