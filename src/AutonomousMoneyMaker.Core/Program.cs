using AutonomousMoneyMaker.Core.Services;
using AutonomousMoneyMaker.Core.Models;

namespace AutonomousMoneyMaker.Core;

class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("🤖 Autonomous Money Maker Starting...");
        
        var moneyMaker = new MoneyMakingEngine();
        var portfolio = new Portfolio();
        
        await moneyMaker.StartAsync(portfolio);
        
        Console.WriteLine("💰 Money making process initiated!");
        Console.WriteLine("Press any key to exit...");
        Console.ReadKey();
    }
}
