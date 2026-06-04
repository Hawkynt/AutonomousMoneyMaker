# CI/CD Pipeline — AutonomousMoneyMaker

Event-driven pipeline (no cron). Workflows live here; their helper scripts live
in `scripts/`.

| File | Trigger | Purpose |
|------|---------|---------|
| `ci.yml` | push + PR on `main` + `workflow_call` | Build the solution and run the test tiers on ubuntu + windows |
| `release.yml` | **manual dispatch** | Build the app, then cut the dated `vyyyyMMdd` Release |
| `nightly.yml` | successful CI on `main` + manual | Publish `nightly-yyyyMMdd` prerelease and prune old ones |
| `_build.yml` | `workflow_call` (internal) | Publish the self-contained Linux app tarball |
| `scripts/version.pl` | invoked by workflows | Stamp each project's own `<Version>` + its folder's commit count (`--stamp`) |
| `scripts/update-changelog.mjs` | invoked by workflows | Bucketise commits into release notes by `+ - * # !` prefix |
| `scripts/prune-nightlies.mjs` | invoked by workflows | GFS retention: 7 daily + 4 weekly + 3 monthly |

## Notes

- **Test tiers.** The five test projects run as tiers in `ci.yml`. Only the
  **Unit** tier is required and blocks a merge; **Integration**, **EndToEnd**,
  **Performance** and **Regression** are advisory (`continue-on-error: true`) —
  they report but never fail CI.
- **No NuGet.** This repo ships an app only; nothing is packed or pushed to
  nuget.org.
- **Versioning — files drive, never tags.** `version.pl --stamp` appends each
  project's folder commit count to its own `<Version>`. There is no single repo
  version, so the repo-level Release/tag is the date marker `vyyyyMMdd`.
- **Releases are manual.** Publishing is a manual dispatch; nightlies and
  changelog notes are automatic.
