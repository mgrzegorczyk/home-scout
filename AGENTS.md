# AGENTS.md

## Project overview

Home.Scout is a self-hosted RSS reader. Users manage a list of RSS channels in a Razor Pages web app (`/RssChannels`) and read the aggregated news feed at `/News`. A background worker fetches every channel every 5 minutes and stores new items in PostgreSQL. Users can also fetch a single channel on demand from the channel list. The whole stack (Postgres 16, web, worker) runs through `docker-compose.yml`.

Stack: .NET 9 (`net9.0`), ASP.NET Core Razor Pages, `BackgroundService` worker, Dapper + Npgsql (no ORM), PostgreSQL 16, Bootstrap 5 + jQuery validation (vendored under `wwwroot/lib`).

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
| RSS fetching | `src/Home.Scout.Application/Rss/RssFeedService.cs`, `RssSyncService.cs` | Feeds are parsed with `System.ServiceModel.Syndication` through a typed `HttpClient` with a 10 s timeout. Sync runs channels in parallel (`MaxDegreeOfParallelism = 6`) and bulk-inserts with duplicate skipping. |
| Background worker | `src/Home.Scout.Background.Rss/RssWorker.cs`, `Program.cs` | Create one DI scope per cycle through `IServiceScopeFactory`. A `PeriodicTimer` fires every 5 min after a 20 s start delay. Catch and log cycle failures so the worker never dies. |
| Config, Docker, environment | `docker-compose.yml`, `.env.example`, `src/*/Dockerfile`, `src/*/appsettings*.json` | The connection string key is `ConnectionStrings:DefaultConnection`, overridden in compose through `ConnectionStrings__DefaultConnection`. `.env` is git-ignored; add any new variable to `.env.example` too. Never commit real secrets. |
| Tests | — | TODO: there are no test projects yet. A new one goes under `tests/` (or `src/`), is added to `Home.Scout.sln` so `dotnet test` picks it up, and the test framework should be agreed first. |
| CI | — | TODO: there are no GitHub Actions workflows yet. The validation gate below runs locally. |

## Validation gate

Run these in order before opening a PR (from `.ai/agentic.config.json`):

1. `dotnet build Home.Scout.sln`
2. `dotnet test Home.Scout.sln --no-build`

## Further reading

- `SDLC.md`: ticket flow, labels, QA gate, claim protocol
- `CODE_REVIEW.md`: review rules for this repository
- `BACKWARD_COMPATIBILITY.md`: protected contract surfaces
- `.ai/agentic.config.json`: agent pipeline configuration
