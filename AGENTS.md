# Agent guide — AutonomousMoneyMaker

Working agreement for **all** coding agents (Claude Code, Codex, Copilot, …)
and human contributors working in this repository. These rules are not
optional. The full house spec lives in the `Hawkynt/project-template` repo
(`STANDARD.md`); this file is the per-repo distillation (it replaces the old
`CLAUDE.md`).

## What this is

A .NET 8 console app **simulating** autonomous investment strategies:
`src/AutonomousMoneyMaker.Core` (Models / Services / Strategies + Program),
`src/AutonomousMoneyMaker.Data` (future data layer), tests under `tests/`.
Run with `dotnet run --project src/AutonomousMoneyMaker.Core`.

## Commits

- **Group changes semantically/logically** — one strategy/service/concern
  per commit.
- **Every subject line starts with a prefix**: `+` added · `-` removed ·
  `*` changed · `#` bug fixed · `!` critical todo.
- Never start a subject with "fix"/"bugfix"/"changed"/"modified".
- **No AI traces anywhere**: no `Co-Authored-By` AI lines, no "Generated
  with" footers, no agent mentions in messages, comments, or authorship.

## The loop (always, in this order)

1. **Before committing**: `dotnet build AutonomousMoneyMaker.sln -c Release`
   and run the unit tier
   (`--filter "TestCategory!=Integration&TestCategory!=EndToEnd&TestCategory!=Performance&TestCategory!=Regression"`)
   until green — the other tiers run advisory in CI.
2. **Commit** (rules above) and **push**.
3. **Wait for CI**; on `main` a green CI triggers the nightly (prerelease +
   GFS prune, same-day replace). Fix and loop until everything is green.

Stable releases are **manual** (`gh workflow run release.yml`) — never cut
one unless explicitly asked.

## Code conventions

- Latest C# features; strategies implement `IInvestmentStrategy` and live in
  `Strategies/` — one class per strategy, risk checks via `RiskManager`,
  never inline in the engine.
- This is a **simulation**: nothing here may ever place real orders or hold
  real credentials; keep the README's disclaimer intact.
- TDD with equivalence classes/boundaries — money math gets exact-decimal
  tests, not float-tolerance ones.

## README & repo conventions

- Standard frame: title → badges → one-line `>` blockquote; fixed emoji
  mapping for the standard sections (`## ❤️ Support`, `## 📜 License`).
- License is LGPL-3.0-or-later; the `## ❤️ Support` section and
  `.github/FUNDING.yml` stay intact.
