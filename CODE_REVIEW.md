# Code review rules

These rules apply to every review in this repository, whether a human or `om-code-review` does it. They add to the built-in checklist; they do not replace it.

## Review priorities

1. **Correctness.** The change does what it claims, including empty, missing, and failure paths.
2. **Security and data safety.** No SQL injection, no secrets in code or config, and no destructive schema change without an upgrade path.
3. **Contracts.** Nothing in `BACKWARD_COMPATIBILITY.md` breaks without its required path.
4. **Layering and conventions.** The change matches how the surrounding code is written (see `AGENTS.md`).
5. **Quality.** Readability and reuse of existing helpers. A review never blocks on this alone.

## Repo-specific checks

### Data access (Dapper + Npgsql)
- Every SQL statement is parameterized through `CommandDefinition` (or Dapper parameters). Any string interpolation or concatenation that puts values into SQL is a **blocker**.
- Identifiers are double-quoted PascalCase (`"RssChannels"`, `"PublishedAt"`) to match the schema. Unquoted identifiers break on Postgres case folding.
- Each repository method opens its own `await using NpgsqlConnection`. Connections are never shared or cached across calls.
- `cancellationToken` reaches every Dapper call through `CommandDefinition`.
- An update or delete that can affect 0 rows checks the affected count, as the existing repositories do.
- Multi-statement writes that must be atomic use a transaction.

### Schema
- A change under `src/Home.Scout.Infrastructure/Data/Scripts/` edits no shipped script. It adds a new numbered script **and** states in the PR how existing databases get it, because init scripts only run on an empty volume. A missing upgrade path is a **blocker**.
- New columns and tables follow the existing style: `TIMESTAMPTZ` for times, `now()` defaults, and explicit indexes for every lookup or sort column.

### Layering
- Domain references nothing. Application references only Domain plus `Microsoft.Extensions.*` abstractions. Npgsql and Dapper appear only in Infrastructure.
- Razor page models and the worker call Application services, never repositories directly.
- New services and repositories are registered in `AddApplication()` / `AddInfrastructure()` with a lifetime that fits: scoped for anything that touches the database.

### Async and cancellation
- `CancellationToken` is accepted and passed down through service → repository → Dapper/HttpClient. A token that is dropped partway (for example an HTTP fetch that ignores it) is a **minor** finding, or **major** when it is in the worker loop.
- No `.Result`, `.Wait()`, or `async void`.
- Code in `Parallel.ForEachAsync` bodies uses the per-iteration token and only thread-safe collections.

### Razor Pages
- User input arrives through a bound `InputModel` with DataAnnotations, and `ModelState.IsValid` is checked before any service call.
- POST handlers keep antiforgery protection. They must not add `[IgnoreAntiforgeryToken]`.
- `KeyNotFoundException` maps to `NotFound()`. Other errors go to `ModelState.AddModelError` and never reach the user as a raw exception page.
- No `@Html.Raw` on feed content. RSS titles and descriptions are untrusted, third-party HTML (XSS).

### RSS fetching and the worker
- Outbound HTTP uses the typed `HttpClient` from DI, never `new HttpClient()`. The 10 s timeout stays bounded.
- A failure in one channel never aborts the whole sync or kills the worker. The worker catches per cycle and logs through `ILogger`.
- Inserting news items stays idempotent: re-fetching a feed must not create duplicates.

### Config and secrets
- Never commit real credentials. A new setting gets a placeholder in `.env.example` and a key in `docker-compose.yml`, and is read from `IConfiguration`.
- Keep nullable warnings at their current level or lower. A new `CS86xx` warning in changed lines is a **minor** finding.

## Severity guidance

| Severity | Meaning | Examples here |
|---|---|---|
| **blocker** | Must fix before merge | SQL injection, a committed secret, an edited shipped schema script or a migration with no upgrade path, a broken build or test, raw feed HTML rendered |
| **major** | Should fix before merge | Repository called from a page model, missing `ModelState` check, a channel failure that kills the sync, duplicate news items on re-fetch |
| **minor** | Fix now or file a follow-up | A dropped `CancellationToken`, a new nullable warning, a missing index on a new lookup column |
| **nit** | Optional | Naming and formatting that differ from the surrounding code |

A PR touching the schema, `docker-compose.yml`, or connection or config keys is `risk-high` (see `SDLC.md`). A PR that changes UI under `src/Home.Scout.Web/Pages` carries `needs-qa`.

## Validation gate

The review runs the configured gate, and any failure is a blocker:

1. `dotnet build Home.Scout.sln`
2. `dotnet test Home.Scout.sln --no-build`
