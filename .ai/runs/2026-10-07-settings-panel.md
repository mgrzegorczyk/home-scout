# Execution plan: Settings panel

Source doc: .ai/specs/2026-10-07-settings-panel.md

## Goal

A gear in the navbar opens a Settings area at `/settings`, with Channels as its first category at `/settings/channels`. The `/RssChannels/*` URLs are removed, and the channel table fits a 360px phone.

## Scope

- Web layer only: the `_Layout` gear, `_SettingsLayout`, the `SettingsCategories` registry, the `/settings` entry page, and the channel pages moved to `Pages/Settings/Channels` with absolute lowercase routes. Hardcoded links are updated. In Phase 2, the channel table gets responsive markup and `site.css` rules.
- `BACKWARD_COMPATIBILITY.md` §3 is amended (owner decision: old routes removed without redirects).
- A new test project, `tests/Home.Scout.Web.Tests` (xUnit + `Microsoft.AspNetCore.Mvc.Testing`, owner-confirmed), runs with faked Application services and no Postgres.

## Non-goals

- Domain, Application, Infrastructure, the worker and the database are untouched.
- No redirects from `/RssChannels/*`, no stored setting values and no other categories.
- The Polish validation messages in `CreateModel` aren't translated.
- The tablet range (768–991px) keeps the horizontal-scroll table (spec Q3).

## Implementation Plan

Follows the spec's Implementation Plan verbatim.

### Phase 1: Settings panel and `/settings` URLs

1. Add the web test project and fixture.
2. Add the category registry.
3. Add the `/settings` entry page.
4. Move the channel pages into the settings panel.
5. Replace the Channels link with the gear.
6. Amend `BACKWARD_COMPATIBILITY.md` §3.

### Phase 2: Phone-friendly channel table

1. Slim the table below `md`.
2. Remove sideways scroll and clipping below `md`.

## Risks

- The dev machine has only the .NET 8 and 10 runtimes. The test project sets `RollForward=Major`, which is a no-op where .NET 9 is installed.
- The 360px criteria (no horizontal scroll, the dropdown not clipped, truncation) can only be verified in a browser. They're pinned by markup tests and checked with `om-auto-qa-pr` through `.ai/scripts/test-env-up.sh`.

## Progress

> Convention: `- [ ]` pending, `- [x]` done. Append ` — <commit sha>` when a step lands. Do not rename step titles.

### Phase 1: Settings panel and `/settings` URLs

- [x] 1.1 Add web test project and fixture — b04a825
- [x] 1.2 Add settings category registry — 6476c70
- [x] 1.3 Add /settings entry page — 3bfbb88 (landed after 1.4: the redirect target page must exist first)
- [x] 1.4 Move channel pages into the settings panel — 9a3e28a
- [x] 1.5 Replace Channels nav link with gear — 5ade416
- [x] 1.6 Amend BACKWARD_COMPATIBILITY.md section 3 — 7526fb9

### Phase 2: Phone-friendly channel table

- [x] 2.1 Slim the channel table below md — fe576f4
- [x] 2.2 Remove sideways scroll and clipping below md — daa6113
