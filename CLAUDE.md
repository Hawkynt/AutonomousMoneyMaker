# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

AutonomousMoneyMaker is a .NET 8 console application that simulates autonomous investment strategies. The application implements a portfolio management system with multiple investment strategies for stocks, ETFs, and cryptocurrencies.

## Solution Structure

```
AutonomousMoneyMaker/
├── AutonomousMoneyMaker.sln
└── src/
    ├── AutonomousMoneyMaker.Core/          # Main console application
    │   ├── Models/                         # Domain models
    │   │   ├── Portfolio.cs               # Portfolio management
    │   │   └── Investment.cs              # Investment entities
    │   ├── Services/                       # Core services
    │   │   ├── MoneyMakingEngine.cs       # Main orchestration
    │   │   ├── MarketDataService.cs       # Market data simulation
    │   │   └── RiskManager.cs             # Risk assessment
    │   ├── Strategies/                     # Investment strategies
    │   │   ├── IInvestmentStrategy.cs     # Strategy interface
    │   │   ├── DiversifiedETFStrategy.cs  # ETF investment logic
    │   │   ├── CryptoTrendStrategy.cs     # Crypto investment logic
    │   │   └── ValueInvestingStrategy.cs  # Value stock strategy
    │   └── Program.cs                      # Application entry point
    └── AutonomousMoneyMaker.Data/          # Data access layer (future use)
```

## Common Commands

- `dotnet build` - Build the entire solution
- `dotnet run --project src/AutonomousMoneyMaker.Core` - Run the application
- `dotnet restore` - Restore NuGet packages
- `dotnet clean` - Clean build artifacts

## Architecture

### Core Components

1. **Portfolio**: Manages investment holdings, cash, and total value tracking
2. **MoneyMakingEngine**: Orchestrates the investment process using multiple strategies
3. **Investment Strategies**: Pluggable strategies implementing `IInvestmentStrategy`
4. **RiskManager**: Evaluates investment risks and enforces limits
5. **MarketDataService**: Simulates market data (mock implementation)

### Investment Strategies

- **DiversifiedETFStrategy**: Focuses on building ETF foundation (target 30% allocation)
- **CryptoTrendStrategy**: Limited crypto exposure with trend analysis (max 10% allocation)
- **ValueInvestingStrategy**: Individual stock picks based on value metrics (up to 40% allocation)

### Risk Management

- Maximum 20% per single investment
- Maximum 40% per investment type
- Maximum 3 investments in same symbol
- Automatic diversification enforcement

## Key Design Patterns

- Strategy Pattern for investment algorithms
- Dependency injection ready architecture
- Async/await for simulated market operations
- SOLID principles adherence

## Testing Structure

The project includes comprehensive test coverage across multiple categories:

### Test Projects

```
tests/
├── AutonomousMoneyMaker.Tests.Unit/          # Unit tests
├── AutonomousMoneyMaker.Tests.Integration/   # Integration tests
├── AutonomousMoneyMaker.Tests.Regression/    # Regression tests
├── AutonomousMoneyMaker.Tests.Performance/   # Performance benchmarks
└── AutonomousMoneyMaker.Tests.EndToEnd/      # End-to-end tests
```

### Test Categories

1. **Unit Tests** - Test individual components in isolation
   - Portfolio management logic
   - Investment calculations
   - Risk management rules
   - Market data service behavior
   - Strategy implementations

2. **Integration Tests** - Test component interactions
   - MoneyMakingEngine workflow
   - Data repository operations
   - Strategy and risk manager integration

3. **Regression Tests** - Ensure backward compatibility
   - Core behavior preservation
   - API contract stability
   - Data precision requirements

4. **Performance Tests** - Benchmark critical operations
   - Portfolio operations at scale
   - Market data retrieval performance
   - Memory usage optimization

5. **End-to-End Tests** - Validate complete workflows
   - Full investment workflow
   - Data persistence scenarios
   - Risk management enforcement
   - Multi-strategy coordination

### Running Tests

- `dotnet test` - Run all tests
- `dotnet test tests/AutonomousMoneyMaker.Tests.Unit` - Run unit tests only
- `dotnet test tests/AutonomousMoneyMaker.Tests.Integration` - Run integration tests
- `dotnet test tests/AutonomousMoneyMaker.Tests.Performance` - Run performance benchmarks

### Test Technologies

- **xUnit** - Testing framework
- **FluentAssertions** - Assertion library for readable tests
- **Moq** - Mocking framework for unit tests
- **BenchmarkDotNet** - Performance benchmarking