namespace AutonomousMoneyMaker.Core.Models;

public class Investment
{
    public string Id { get; set; }
    public string Symbol { get; set; }
    public string Name { get; set; }
    public decimal Amount { get; set; }
    public decimal CurrentValue { get; set; }
    public DateTime PurchaseDate { get; set; }
    public InvestmentType Type { get; set; }

    public Investment(string symbol, string name, decimal amount, InvestmentType type)
    {
        Id = Guid.NewGuid().ToString();
        Symbol = symbol;
        Name = name;
        Amount = amount;
        CurrentValue = amount;
        PurchaseDate = DateTime.UtcNow;
        Type = type;
    }

    public decimal GetProfitLoss()
    {
        return CurrentValue - Amount;
    }

    public decimal GetProfitLossPercentage()
    {
        return Amount == 0 ? 0 : (GetProfitLoss() / Amount) * 100;
    }
}

public enum InvestmentType
{
    Stock,
    Crypto,
    Bond,
    ETF,
    Commodity,
    RealEstate
}