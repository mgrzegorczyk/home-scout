# Home.Scout

A self-hosted RSS reader. Home.Scout pulls news from any number of RSS channels, stores every item in PostgreSQL, and shows them as one aggregated news list.

It is also an experiment in **AI-assisted software delivery**. The application is built with [Claude Code](https://claude.com/claude-code) and the [Open Mercato skills](https://github.com/open-mercato/skills), following a documented SDLC: briefs, specs, issues, pull requests, automated review and QA. The repository history is meant to show what that way of working looks like on a real project, step by step.

## Features

- **Channel management**: add, edit and delete RSS channels under `/settings/channels`.
- **Background fetching**: a worker fetches every channel every 5 minutes and stores only new items (duplicates are skipped).
- **Fetch on demand**: refresh a single channel straight from the channel list.
- **News feed**: all items from all channels in one list at `/news` (the home page).
- **Phone-friendly**: the settings panel and channel table work on narrow screens without sideways scrolling.

## Tech stack

| Area | Technology |
|---|---|
| Runtime | .NET 9 |
| Web UI | ASP.NET Core Razor Pages, Bootstrap 5, jQuery Validation |
| Background jobs | .NET `BackgroundService` worker with a `PeriodicTimer` |
| RSS parsing | `System.ServiceModel.Syndication` |
| Data access | Dapper + Npgsql (no ORM, plain SQL) |
| Database | PostgreSQL 16 |
| Tests | xUnit + `Microsoft.AspNetCore.Mvc.Testing` |
| Hosting | Docker Compose |

## Architecture

The solution follows a layered (clean architecture) layout, with dependencies pointing inward only:

```
Domain  ←  Application  ←  Infrastructure
                ↑                ↑
          Web (Razor Pages), Background.Rss (worker)
```

| Project | Responsibility |
|---|---|
| `Home.Scout.Domain` | Plain domain models, no dependencies |
| `Home.Scout.Application` | Use cases: channel and news services, RSS fetching and sync, repository interfaces |
| `Home.Scout.Infrastructure` | Dapper repositories, SQL schema scripts |
| `Home.Scout.Web` | Razor Pages UI (news feed, settings panel) |
| `Home.Scout.Background.Rss` | Worker that syncs all channels every 5 minutes |
| `tests/Home.Scout.Web.Tests` | In-memory integration tests for the web app |

Docker Compose runs three containers: `db` (PostgreSQL), `web`, and `background-rss-worker`.

## Getting started

### Requirements

- [Docker](https://www.docker.com/) with Docker Compose
- [.NET 9 SDK](https://dotnet.microsoft.com/download) (only for local development and tests)

### Run with Docker

```bash
git clone https://github.com/mgrzegorczyk/home-scout.git
cd home-scout

cp .env.example .env      # then set your own DB_PASSWORD (in DB_CONNECTION_STRING too)
docker compose up -d --build
```

Open <http://localhost:8080>, go to **Settings → Channels**, and add your first RSS feed. The worker picks it up within 5 minutes, or you can click **Fetch now** right away.

The database schema is created automatically from `src/Home.Scout.Infrastructure/Data/Scripts/` the first time the `postgres_data` volume is initialized.

### Configuration

All settings live in `.env` (see `.env.example`):

| Variable | Purpose |
|---|---|
| `DB_NAME`, `DB_USER`, `DB_PASSWORD` | PostgreSQL database and credentials |
| `DB_CONNECTION_STRING` | Connection string used by the web app and the worker |
| `DB_PORT` | Host port that exposes PostgreSQL (default `1436`) |
| `WEB_PORT` | Host port for the web app (default `8080`) |
| `ASPNETCORE_ENVIRONMENT` | ASP.NET Core environment name |

`.env` is git-ignored. Never commit real credentials.

### Build and test

```bash
dotnet build Home.Scout.sln
dotnet test Home.Scout.sln --no-build
```

These two commands are also the validation gate every pull request must pass.

## How this project is built

Home.Scout is developed with AI agents, not just with AI autocomplete. Claude Code drives the work through the Open Mercato skill set (`om-*` skills), and the process is written down in the repository so both humans and agents follow the same rules.

A typical feature goes like this:

1. **Brainstorm and brief**: an idea is talked through and captured as a brief (`.ai/specs/briefs/`).
2. **Spec**: the brief becomes a design document with resolved open questions and an implementation plan (`.ai/specs/`), shipped as its own design-only PR.
3. **Implementation**: an agent implements the spec in an isolated worktree, tracking progress in a run plan (`.ai/runs/`), and opens a labeled PR.
4. **Review**: the PR is reviewed against the repository's review rules, and requested changes are fixed and re-reviewed.
5. **QA**: user-facing changes are checked in a real browser against a local test environment (`.ai/scripts/test-env-up.sh`).
6. **Merge**: the PR is squash-merged once every gate passes.

The owner stays in the loop: product decisions, spec confirmations and final approvals are human calls, and the PR discussions record them.

### Process documents

| File | What it covers |
|---|---|
| [`AGENTS.md`](AGENTS.md) | Project map and coding rules for agents |
| [`SDLC.md`](SDLC.md) | Ticket lifecycle, labels, QA gate, claim protocol |
| [`CODE_REVIEW.md`](CODE_REVIEW.md) | Review rules for this repository |
| [`BACKWARD_COMPATIBILITY.md`](BACKWARD_COMPATIBILITY.md) | Protected contract surfaces |
| [`.ai/agentic.config.json`](.ai/agentic.config.json) | Agent pipeline configuration |

To follow the experiment, read the closed pull requests together with the matching files in `.ai/specs/` and `.ai/runs/`.

## Status

Early development (`v0.1`). Expect breaking changes. There is no migration runner yet, so schema changes may require recreating the database volume.
