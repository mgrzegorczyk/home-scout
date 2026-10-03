# Backward compatibility

This file lists the surfaces this project treats as protected contracts, what counts as breaking each one, and the required path when a break is intentional. Review skills check every change against it, and implementation skills warn when a change does not comply.

Home.Scout is a single deployable app (web, worker, and database shipped together through `docker-compose.yml`). It publishes no NuGet packages, public HTTP API, CLI, or events. Its contracts are the **persisted data** and the **operator-facing deployment surface**: anyone running it has an existing Postgres volume and a `.env` file that must keep working after an upgrade.

## 1. Database schema and data — highest risk

**Surface:** the `RssChannels` and `NewsItems` tables, defined by `src/Home.Scout.Infrastructure/Data/Scripts/*.sql`, plus the data in existing volumes.

**How scripts run today:** the folder is mounted into `docker-entrypoint-initdb.d`, so scripts execute **only when the volume is empty**. There is no migration runner. An existing installation never sees a new or edited script.

**Breaking:** dropping or renaming a table, column, or index; narrowing a type or adding `NOT NULL` without a default; changing `ON DELETE CASCADE` behaviour; editing a script that has already shipped; any change that makes existing rows invalid.

**Required path:**
- Never edit a shipped script. Add the next numbered script (`002_*.sql`, …) and make it idempotent (`IF NOT EXISTS`, `ADD COLUMN IF NOT EXISTS`).
- The PR states how existing databases get the change: a documented manual `psql` step, or a migration runner introduced in that same PR.
- Prefer additive changes (expand, then contract): add the new column or table, ship code that reads both, and remove the old one in a later release.
- The PR body includes a rollback plan. Label it `risk-high`.

## 2. Deployment configuration

**Surface:** environment variables in `.env.example` and `docker-compose.yml` (`DB_NAME`, `DB_USER`, `DB_PASSWORD`, `DB_CONNECTION_STRING`, `DB_PORT`, `WEB_PORT`, `ASPNETCORE_ENVIRONMENT`); the configuration key `ConnectionStrings:DefaultConnection`; compose service names (`db`, `web`, `background-rss-worker`), the `postgres_data` volume name, and the web container port `8080`.

**Breaking:** renaming or removing a variable or key; making a new variable mandatory with no default; renaming the volume, which orphans existing data; changing the published port mapping.

**Required path:** keep reading the old name as a fallback for at least one release and log a warning when it is used. Add every new variable to `.env.example` with a safe default. Describe the change under an upgrade note in the PR body and the changelog.

## 3. Web routes

**Surface:** the user-visible URLs `/` (redirects to `/News`), `/News`, `/RssChannels`, `/RssChannels/Create`, `/RssChannels/Edit/{id:int}`, `/RssChannels/Delete/{id:int}`, and the `FetchNow` handler on `/RssChannels`. Users bookmark these.

**Breaking:** removing or renaming a route, or changing its parameter shape.

**Required path:** keep the old URL working with a redirect to the new one, and mention the change in the PR. Internal page model and handler refactors that keep the URLs are not breaking.

## 4. Background sync behaviour

**Surface:** the worker refreshes every channel periodically (currently every 5 min after a 20 s start delay), and re-fetching never creates duplicate news items.

**Breaking:** losing the de-duplication guarantee; stopping automatic refresh; changing the interval by an order of magnitude without making it configurable.

**Required path:** call the behaviour change out in the PR and the changelog. Make timing changes configurable, with the old value as the default.

## Not protected

These are internal and may change freely when the build and tests stay green: C# interfaces, services, DTOs, and repositories (there are no external consumers); Razor markup and CSS (UI changes go through QA, not this file); and vendored `wwwroot/lib` versions, provided the UI keeps working.
