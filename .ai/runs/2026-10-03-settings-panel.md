# Execution plan: Settings panel

Source doc: .ai/specs/2026-10-03-settings-panel.md
Refs: #4 (spec PR, design-only) · Closes: #1

## Goal

Replace the top-level `Channels` navbar link with a gear icon that opens a Settings area (left category menu, `Channels` as the first category), keeping every `/RssChannels/*` route unchanged.

## Scope

- Web layer only: `_Layout`, new `_SettingsLayout`, new `Pages/Settings/Index` (302 to the first category), `Settings/SettingsCategories` registry, `Pages/RssChannels/_ViewStart.cshtml`, and the four channel views lose their outer `container py-4` wrapper.
- New test project `tests/Home.Scout.Web.Tests` (xUnit + `Microsoft.AspNetCore.Mvc.Testing`), added to `Home.Scout.sln`. Services are faked through `ConfigureTestServices`, so Postgres isn't needed.

## Non-goals

- No change to Domain, Application, Infrastructure, the worker or the database.
- No URL change for the channel pages (`BACKWARD_COMPATIBILITY.md` §3) and no stored setting values.
- The pre-existing broken navbar brand link (`/Index`) stays as is.

## Implementation Plan

### Phase 1: Foundations

1. Add the web test project with a `WebApplicationFactory<Program>` fixture that fakes `IChannelService`, `IRssSyncService` and `INewsItemService`, expose `public partial class Program`, and add a smoke test.
2. Add the `SettingsCategories` registry with `FindForPage` (exact prefix or prefix + `/`) and unit tests.
3. Add the `/Settings` entry page (302 to the first category, 404 when the registry is empty) and an integration test.

### Phase 2: Settings panel

1. Add `_SettingsLayout` (heading, `<nav aria-label="Settings categories">`, re-rendered `Scripts` section), `RssChannels/_ViewStart.cshtml`, drop the duplicated wrappers, and add integration tests.
2. Replace the `Channels` nav link with the right-aligned gear (`aria-label`, visually-hidden text, `title`, active state from the registry) and add integration tests.

## Risks

- The dev machine has no .NET 9 runtime (only 8 and 10). The test project sets `RollForward=Major`, so it runs on the next available major when 9 is missing and stays on 9 where it's installed. Production projects are untouched.
- Bootstrap Icons comes from a CDN. The gear keeps a visually-hidden label and a `title` as fallback (spec risk review).

## Progress

> Convention: `- [ ]` pending, `- [x]` done. Append ` — <commit sha>` when a step lands. Do not rename step titles.

### Phase 1: Foundations

- [x] 1.1 Add web test project and fixture — af59e5c
- [x] 1.2 Add settings category registry — 57bbe55
- [x] 1.3 Add /Settings entry page — b582e4c

### Phase 2: Settings panel

- [x] 2.1 Add _SettingsLayout and move channel pages into it — 719f098
- [x] 2.2 Replace Channels nav link with gear — 7c539f3
- [x] Post-review fix: gear switches to bi-gear-fill on settings pages (text-dark hid .active) — ad7a8ba
