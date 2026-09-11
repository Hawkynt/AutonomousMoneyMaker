# AutonomousMoneyMaker

[![License](https://img.shields.io/github/license/Hawkynt/AutonomousMoneyMaker)](https://github.com/Hawkynt/AutonomousMoneyMaker/blob/main/LICENSE)
[![Language](https://img.shields.io/github/languages/top/Hawkynt/AutonomousMoneyMaker?color=8957D5)](https://github.com/Hawkynt/AutonomousMoneyMaker)

[![CI](https://github.com/Hawkynt/AutonomousMoneyMaker/actions/workflows/ci.yml/badge.svg?branch=main)](https://github.com/Hawkynt/AutonomousMoneyMaker/actions/workflows/ci.yml)
![Last Commit](https://img.shields.io/github/last-commit/Hawkynt/AutonomousMoneyMaker?branch=main)
![Activity](https://img.shields.io/github/commit-activity/m/Hawkynt/AutonomousMoneyMaker)

[![Stars](https://img.shields.io/github/stars/Hawkynt/AutonomousMoneyMaker?color=FFD700)](https://github.com/Hawkynt/AutonomousMoneyMaker/stargazers)
[![Forks](https://img.shields.io/github/forks/Hawkynt/AutonomousMoneyMaker?color=008080)](https://github.com/Hawkynt/AutonomousMoneyMaker/network/members)
[![Issues](https://img.shields.io/github/issues/Hawkynt/AutonomousMoneyMaker)](https://github.com/Hawkynt/AutonomousMoneyMaker/issues)
![Code Size](https://img.shields.io/github/languages/code-size/Hawkynt/AutonomousMoneyMaker?color=4CAF50)
![Repo Size](https://img.shields.io/github/repo-size/Hawkynt/AutonomousMoneyMaker?color=FF9800)

[![Release](https://img.shields.io/github/v/release/Hawkynt/AutonomousMoneyMaker)](https://github.com/Hawkynt/AutonomousMoneyMaker/releases/latest)
[![Nightly](https://img.shields.io/github/v/release/Hawkynt/AutonomousMoneyMaker?include_prereleases&sort=date&filter=nightly-*&label=nightly&color=FF9800)](https://github.com/Hawkynt/AutonomousMoneyMaker/releases)
[![Downloads](https://img.shields.io/github/downloads/Hawkynt/AutonomousMoneyMaker/total)](https://github.com/Hawkynt/AutonomousMoneyMaker/releases)

> This is entirely written by AI agents. I have no idea what they came up with, but maybe it will work.

## 🧭 Vision

An experiment in whether a set of mechanical investment strategies, written down as code and
backtested honestly, does better than guessing. Three strategies are implemented behind one
interface — value investing, a diversified ETF allocation, and a crypto trend follower — so they can
be compared on the same data rather than argued about.

Nothing here places a real order. The value, if there is any, is in being able to test a strategy
before believing it, which is the opposite of how most of them are adopted.

## ✨ Features

- Three strategies behind one `IInvestmentStrategy` interface — value investing, diversified ETF allocation and crypto trend following
- A separate data layer, so a strategy is evaluated against the same series regardless of where it came from
- Test suites split by kind: unit, integration, regression, performance and end-to-end

## 📦 Installation

Clone the repository and build it — see [Building](#-building). There is no released binary, and
deliberately so: this is a study, not a product.

## 🚀 Quick start

```bash
dotnet run --project src/AutonomousMoneyMaker.Core
```

## 🎯 Purpose

Should make me rich

## ⚠️ Limitations

**TRADING INVOLVES SUBSTANTIAL RISK OF LOSS**

- Past performance is not indicative of future results
- Only trade with money you can afford to lose
- This software is provided "as-is" without warranty
- The author is not responsible for any trading losses
- Always test thoroughly in demo accounts first
- Consider seeking advice from qualified financial advisors

## 🛠️ Building

```bash
dotnet build -c Release
dotnet test
```

## 🤝 Contributing

- ⭐ **Star this repository** if you find it useful
- 🐛 **Report bugs** and suggest improvements
- 📖 **Contribute documentation** or code improvements

## ❤️ Support

If this project saves you time or money, consider supporting its development:

[![GitHub Sponsors](https://img.shields.io/badge/GitHub-Sponsor-EA4AAA?logo=githubsponsors)](https://github.com/sponsors/Hawkynt)
[![PayPal](https://img.shields.io/badge/PayPal-Donate-00457C?logo=paypal)](https://www.paypal.me/hawkynt)

## 📜 License

Licensed under LGPL-3.0-or-later — see [LICENSE](LICENSE).
