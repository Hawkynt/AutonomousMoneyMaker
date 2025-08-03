using AutonomousMoneyMaker.Core.Models;
using AutonomousMoneyMaker.Core.Strategies;

namespace AutonomousMoneyMaker.Core.Services;

public class RiskManager
{
    private const decimal MaxSingleInvestmentPercentage = 0.20m; // 20% max per investment
    private const decimal MaxRiskPercentage = 0.15m; // 15% max risk tolerance

    public bool IsAcceptableRisk(InvestmentRecommendation recommendation, Portfolio portfolio)
    {
        // Check if investment amount is within risk limits
        var investmentPercentage = recommendation.Amount / portfolio.TotalValue;
        if (investmentPercentage > MaxSingleInvestmentPercentage)
        {
            return false;
        }

        // Check diversification
        var sameTypeInvestments = portfolio.Investments
            .Where(i => i.Type == recommendation.Type)
            .Sum(i => i.CurrentValue);
            
        var typePercentage = (sameTypeInvestments + recommendation.Amount) / portfolio.TotalValue;
        if (typePercentage > 0.40m) // Max 40% in same type
        {
            return false;
        }

        // Check if we have too many investments in the same symbol
        var sameSymbolCount = portfolio.Investments.Count(i => i.Symbol == recommendation.Symbol);
        if (sameSymbolCount >= 3) // Max 3 investments in same symbol
        {
            return false;
        }

        return true;
    }

    public decimal CalculateRiskScore(Portfolio portfolio)
    {
        if (!portfolio.Investments.Any())
            return 0;

        var volatilityScore = CalculateVolatilityScore(portfolio);
        var diversificationScore = CalculateDiversificationScore(portfolio);
        
        return (volatilityScore + diversificationScore) / 2;
    }

    private decimal CalculateVolatilityScore(Portfolio portfolio)
    {
        var cryptoPercentage = portfolio.Investments
            .Where(i => i.Type == InvestmentType.Crypto)
            .Sum(i => i.CurrentValue) / portfolio.TotalValue;
            
        return cryptoPercentage * 10; // Crypto is considered 10x more volatile
    }

    private decimal CalculateDiversificationScore(Portfolio portfolio)
    {
        var typeGroups = portfolio.Investments.GroupBy(i => i.Type);
        var diversificationRatio = (decimal)typeGroups.Count() / Enum.GetValues<InvestmentType>().Length;
        
        return (1 - diversificationRatio) * 5; // Less diversification = higher risk
    }
}