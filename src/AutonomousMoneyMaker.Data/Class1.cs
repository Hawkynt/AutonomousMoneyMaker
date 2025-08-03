using AutonomousMoneyMaker.Data.Models;

namespace AutonomousMoneyMaker.Data;

public interface IPortfolioRepository
{
    Task SavePortfolioAsync(PortfolioData portfolio);
    Task<PortfolioData?> LoadPortfolioAsync(string portfolioId);
    Task<List<PortfolioData>> GetAllPortfoliosAsync();
    Task DeletePortfolioAsync(string portfolioId);
}

public interface IInvestmentHistoryRepository
{
    Task SaveInvestmentAsync(InvestmentHistoryData investment);
    Task<List<InvestmentHistoryData>> GetInvestmentHistoryAsync(string portfolioId);
    Task<List<InvestmentHistoryData>> GetInvestmentsBySymbolAsync(string symbol);
}
