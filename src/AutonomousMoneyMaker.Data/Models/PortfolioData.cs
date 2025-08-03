namespace AutonomousMoneyMaker.Data.Models;

public class PortfolioData
{
    public string Id { get; set; } = string.Empty;
    public decimal TotalValue { get; set; }
    public decimal AvailableCash { get; set; }
    public DateTime LastUpdated { get; set; }
    public List<InvestmentData> Investments { get; set; } = new();
}

public class InvestmentData
{
    public string Id { get; set; } = string.Empty;
    public string Symbol { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public decimal CurrentValue { get; set; }
    public DateTime PurchaseDate { get; set; }
    public string InvestmentType { get; set; } = string.Empty;
}