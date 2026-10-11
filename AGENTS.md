# AGENTS.md

## Project overview

Home.Scout is a self-hosted RSS reader. Users manage a list of RSS channels in the Razor Pages settings panel (`/settings/channels`, opened from the gear icon in the navbar) and read the aggregated news feed at `/News`. A background worker fetches every channel every 5 minutes and stores new items in PostgreSQL. Users can also fetch a single channel on demand from the channel list. The whole stack (Postgres 16, web, worker) runs through `docker-compose.yml`.

Stack: .NET 9 (`net9.0`), ASP.NET Core Razor Pages, `BackgroundService` worker, Dapper + Npgsql (no ORM), PostgreSQL 16, Bootstrap 5 + jQuery validation (vendored under `wwwroot/lib`), xUnit + `Microsoft.AspNetCore.Mvc.Testing` for tests.

The repository is also an experiment in AI-assisted delivery: work goes through the Open Mercato `om-*` skills and the process in `SDLC.md` (brief → spec → PR → review → QA → merge). Specs and briefs live in `.ai/specs/`, run plans in `.ai/runs/`.

Layering (dependencies point inward only):

```
Domain  ←  Application  ←  Infrastructure
                ↑                ↑
          Web (Razor Pages), Background.Rss (worker)
```

## Task routing

| When the task involves… | Read first | Key rules |
|---|---|---|
| Domain models | `src/Home.Scout.Domain/Models/` | Plain POCOs with no package references. Use `int Id`, initialize strings to `string.Empty`, and store timestamps as UTC `DateTime`. |
| Business logic / use cases | `src/Home.Scout.Application/` (`RssChannels/ChannelService.cs`, `News/NewsItemService.cs`, `Rss/`), `DependencyInjection.cs` | Declare the service interface in the same file as its implementation. Validate input in the service: throw `ArgumentException` for bad input and `KeyNotFoundException` for a missing entity. Trim user strings and set timestamps with `DateTime.UtcNow`. Map entities to DTOs (`*/Dto/`) with a private static `MapToDto`. Register services in `AddApplication()`. |
| Data access / SQL | `src/Home.Scout.Infrastructure/*/…Repository.cs`, `Data/Scripts/001_Initial_Schema.sql`, `DependencyInjection.cs` | Repository interfaces live in Application and are implemented here. Open one `await using NpgsqlConnection` per method. Write SQL as `const` raw string literals with double-quoted PascalCase identifiers. Always parameterize through `CommandDefinition(sql, params, cancellationToken: …)`. Throw `InvalidOperationException` when an update or delete affects 0 rows. Register repositories in `AddInfrastructure()`. |
| Database schema | `src/Home.Scout.Infrastructure/Data/Scripts/`, `docker-compose.yml` (`db` service) | Scripts run only through `docker-entrypoint-initdb.d`, so only on an empty volume, and there is no migration runner. Never edit a shipped script; add a numbered `00N_*.sql` and an upgrade path for existing databases (see `BACKWARD_COMPATIBILITY.md`). |
| Web UI (Razor Pages) | `src/Home.Scout.Web/Pages/`, `Pages/Shared/_Layout.cshtml`, `Program.cs`, `wwwroot/css/site.css` | Each page binds an `InputModel` (DataAnnotations) through `[BindProperty]` and checks `ModelState.IsValid`. Map `KeyNotFoundException` to `NotFound()` and surface other errors with `ModelState.AddModelError`. Page models call Application services only, never repositories. `/` redirects to `/News`. |
| Settings panel | `src/Home.Scout.Web/Settings/SettingsCategories.cs`, `Pages/Settings/`, `Pages/Shared/_SettingsLayout.cshtml`, `.ai/specs/2026-10-07-settings-panel.md` | Every configuration screen lives under `/settings/`. Add a category with one entry in `SettingsCategories.All` (its `PagePrefix` is a Razor page path such as `/Settings/Channels`, not a URL) plus pages under `Pages/Settings/{Category}/`. Give each page an explicit lowercase absolute route (`@page "/settings/channels/edit/{id:int}"`) instead of turning on `LowercaseUrls`. `/settings` redirects to the first category. `_SettingsLayout` must keep rendering `@section Scripts`, because Create and Edit pages need it for client-side validation. The old `/RssChannels/*` URLs are removed and return 404. |
| RSS fetching | `src/Home.Scout.Application/Rss/RssFeedService.cs`, `RssSyncService.cs` | Feeds are parsed with `System.ServiceModel.Syndication` through a typed `HttpClient` with a 10 s timeout. Sync runs channels in parallel (`MaxDegreeOfParallelism = 6`) and bulk-inserts with duplicate skipping. |
| Background worker | `src/Home.Scout.Background.Rss/RssWorker.cs`, `Program.cs` | Create one DI scope per cycle through `IServiceScopeFactory`. A `PeriodicTimer` fires every 5 min after a 20 s start delay. Catch and log cycle failures so the worker never dies. |
| Config, Docker, environment | `docker-compose.yml`, `.env.example`, `src/*/Dockerfile`, `src/*/appsettings*.json` | The connection string key is `ConnectionStrings:DefaultConnection`, overridden in compose through `ConnectionStrings__DefaultConnection`. `.env` is git-ignored; add any new variable to `.env.example` too. Never commit real secrets. |
| Tests | `tests/Home.Scout.Web.Tests/` (`ScoutWebApplicationFactory.cs`, `Fakes/`) | xUnit + `Microsoft.AspNetCore.Mvc.Testing`. Web tests host the app in memory through `ScoutWebApplicationFactory`, which swaps the Application services for fakes from `Fakes/`, so no Postgres is needed. Assert on routes, status codes and rendered HTML, and use `CreateNonRedirectingClient()` to check redirects. A new test project goes under `tests/` and must be added to `Home.Scout.sln` so `dotnet test` picks it up. Application, Infrastructure and the worker have no tests yet. |
| Local QA environment | `.ai/scripts/test-env-up.sh`, `.ai/scripts/test-env-down.sh`, `.ai/qa/test-env.json` | Start the app for browser QA (`om-prepare-test-env`, `om-auto-qa-pr`) through these scripts instead of bringing up a second stack by hand. `.ai/qa/` holds generated state and logs. |
| CI | — | TODO: there are no GitHub Actions workflows yet. The validation gate below runs locally. |

## Validation gate

Run these in order before opening a PR (from `.ai/agentic.config.json`):

1. `dotnet build Home.Scout.sln`
2. `dotnet test Home.Scout.sln --no-build`

## Further reading

- `README.md`: public project description and quick start; update it when features, setup or the process change
- `SDLC.md`: ticket flow, labels, QA gate, claim protocol
- `CODE_REVIEW.md`: review rules for this repository
- `BACKWARD_COMPATIBILITY.md`: protected contract surfaces
- `.ai/agentic.config.json`: agent pipeline configuration
