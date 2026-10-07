# Execution plan: Commit the test environment scripts

## Goal

Commit the verified `om-prepare-test-env` entrypoint scripts so every checkout and worktree, including the ones QA skills create, can boot the app with its own disposable database in one command.

## Scope

- `.ai/scripts/test-env-up.sh` and `.ai/scripts/test-env-down.sh`. These are generated and were verified locally: cold 82s including the Docker Desktop start, warm 6s reused, 62s rebuild after a source change, and teardown is idempotent.
- `.gitattributes`, which forces LF on `*.sh` so Windows checkouts (`core.autocrlf`) can't break `sh` with `\r`.
- `.gitignore`, which ignores the per-run environment state (build cache, logs, the started marker, the lock).
- `.ai/skills/om-prepare-test-env/SKILL.md`, the repo-local notes holding the working command chain and the lessons behind the script's choices.

## Non-goals

- No application code, tests, or `docker-compose.yml` changes.
- No PowerShell (`.ps1`) flavor. Git Bash runs the `.sh` scripts on Windows.
- No database seed command.

## Implementation Plan

### Phase 1: Test environment tooling

1. Add the entrypoint scripts with LF line-ending rules.
2. Ignore the runtime state and add the repo-local skill notes.

## Risks

- The scripts are Windows/Git Bash-verified only. The macOS/Linux branches (`stat -f`, `open -a Docker`) follow the skill's portable contract but haven't been run here.
- The scripts assume Docker is available. On Windows and macOS they start Docker Desktop when the daemon is down.

## Progress

> Convention: `- [ ]` pending, `- [x]` done. Append ` — <commit sha>` when a step lands. Do not rename step titles.

### Phase 1: Test environment tooling

- [x] 1.1 Add the entrypoint scripts with LF line-ending rules — 5b9591a
- [x] 1.2 Ignore runtime state and add repo-local skill notes — 15e3ed0
- [x] Post-review fix: portable descriptor update in teardown (BSD sed) — 6a95dea
