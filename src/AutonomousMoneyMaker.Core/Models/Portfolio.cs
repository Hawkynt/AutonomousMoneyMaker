namespace AutonomousMoneyMaker.Core.Models;

public class Portfolio
{
    public decimal TotalValue { get; private set; }
    public decimal AvailableCash { get; private set; }
    public List<Investment> Investments { get; private set; }
    public DateTime LastUpdated { get; private set; }

    public Portfolio(decimal initialCash = 1000m)
    {
        AvailableCash = initialCash;
        TotalValue = initialCash;
        Investments = new List<Investment>();
        LastUpdated = DateTime.UtcNow;
    }

    public bool CanInvest(decimal amount)
    {
        return AvailableCash >= amount;
    }

    public void AddInvestment(Investment investment)
    {
        if (!CanInvest(investment.Amount))
            throw new InvalidOperationException("Insufficient funds for investment");

        AvailableCash -= investment.Amount;
        Investments.Add(investment);
        UpdateTotalValue();
    }

    public void UpdateInvestmentValue(string investmentId, decimal newValue)
    {
        var investment = Investments.FirstOrDefault(i => i.Id == investmentId);
        if (investment != null)
        {
            investment.CurrentValue = newValue;
            UpdateTotalValue();
        }
    }

    private void UpdateTotalValue()
    {
        TotalValue = AvailableCash + Investments.Sum(i => i.CurrentValue);
        LastUpdated = DateTime.UtcNow;
    }

    public decimal GetTotalProfit()
    {
        return Investments.Sum(i => i.CurrentValue - i.Amount);
    }
}