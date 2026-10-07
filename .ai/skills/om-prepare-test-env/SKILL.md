---
name: om-prepare-test-env
description: Repo-local notes for om-prepare-test-env in Home.Scout. Extends the installed skill; its rules still apply.
---

# om-prepare-test-env: Home.Scout notes

The entrypoint is `.ai/scripts/test-env-up.sh` (POSIX `sh`; on Windows run it from Git Bash), and teardown is `.ai/scripts/test-env-down.sh`. Re-invoking the skill just runs the script.

## Working command chain (what the script does)

1. `dotnet build src/Home.Scout.Web/Home.Scout.Web.csproj -c Debug`. This is skipped while the build-cache fingerprint of `src/` and `Home.Scout.sln` is unchanged.
2. A disposable `postgres:16-alpine` container named `home-scout-qa-db-<checkout hash>` on `127.0.0.1:15432` (or a random 20000–40000 port when that one is taken or reserved). `src/Home.Scout.Infrastructure/Data/Scripts` is mounted into `docker-entrypoint-initdb.d`, exactly as `docker-compose.yml` does.
3. Wait for `pg_isready -h 127.0.0.1` **and** for the `"RssChannels"` table to exist. The init scripts run on a socket-only temporary server first, so plain `pg_isready` passes too early.
4. `dotnet bin/Debug/net9.0/Home.Scout.Web.dll`, run from `src/Home.Scout.Web` (that's the content root for `wwwroot`), with `ASPNETCORE_ENVIRONMENT=Development`, `DOTNET_ROLL_FORWARD=Major`, `ASPNETCORE_URLS=http://127.0.0.1:5299` and `ConnectionStrings__DefaultConnection`. The health probe is `/News` (200 only when the database answers).

## Lessons (each one prevents a known failure)

- **Don't use `docker-compose.yml` for QA.** It hardcodes the container names (`home-scout-db`, …), so it collides with the developer's own stack, and the `web` image rebuild takes minutes.
- **Don't use `dotnet run`.** On Windows it leaves a child `Home.Scout.Web.exe` alive after the parent is killed. Running the built dll gives one process. On Git Bash the script records `/proc/<pid>/winpid` and stops it with `taskkill`.
- **Set `DOTNET_ROLL_FORWARD=Major`.** The project targets net9.0, and the dev machine has only the .NET 8 and 10 runtimes.
- **Use the port retry.** Windows reserves ranges such as 55xxx (Hyper-V/WinNAT), so `docker run -p 127.0.0.1:55432` fails with "access permissions". The script retries on random 20000–40000 ports.
- **Let the script start Docker Desktop.** When the daemon is down, it starts Docker Desktop (Windows/macOS) and waits up to 180s (2026-10-07: the first cold run failed with "docker is not running").
- **`python` is not installed.** `free_port` falls back to `node`.
- **The database starts empty.** There is no seed command in the repo, so QA creates channels through the UI. The RSS worker is not started; QA uses "Fetch now".
