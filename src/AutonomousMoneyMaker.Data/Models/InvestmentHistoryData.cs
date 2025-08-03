namespace AutonomousMoneyMaker.Data.Models;

public class InvestmentHistoryData
{
    public string Id { get; set; } = string.Empty;
    public string PortfolioId { get; set; } = string.Empty;
    public string Symbol { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public decimal PurchasePrice { get; set; }
    public DateTime PurchaseDate { get; set; }
    public string InvestmentType { get; set; } = string.Empty;
    public string Strategy { get; set; } = string.Empty;
    public string Reasoning { get; set; } = string.Empty;
    public decimal ConfidenceScore { get; set; }
}