#!/bin/sh
# om-prepare-test-env: generated entrypoint (contract v2) — teardown
# regenerate with: om-prepare-test-env --regenerate
# history:
#   2026-10-07 generated: stops the app and removes the checkout-scoped Postgres container
#   2026-10-07 repair: portable descriptor update instead of sed -i (BSD sed aborted teardown before the container was removed)
set -eu

ROOT=$(cd "$(dirname "$0")/../.." && pwd)
cd "$ROOT"
DESCRIPTOR=".ai/qa/test-env.json"
SCOPE=$(printf '%s' "$ROOT" | cksum | awk '{print $1}')
DB_CONTAINER="home-scout-qa-db-$SCOPE"

json_get() { sed -n "s/.*\"$1\": *\"\{0,1\}\([^\",}]*\)\"\{0,1\}.*/\1/p" "$2" 2>/dev/null | head -n 1; }

if [ -f "$DESCRIPTOR" ] && [ "$(json_get startedByThisRepo "$DESCRIPTOR")" = true ]; then
  pid=$(json_get pid "$DESCRIPTOR"); win=$(json_get winPid "$DESCRIPTOR")
  [ -n "$pid" ] && kill "$pid" 2>/dev/null || true
  if [ -n "$win" ] && command -v taskkill >/dev/null 2>&1; then taskkill //PID "$win" //F >/dev/null 2>&1 || true; fi
  # portable in-place edit (BSD sed on macOS needs an argument after -i)
  sed 's/"status": *"running"/"status": "stopped"/' "$DESCRIPTOR" > "$DESCRIPTOR.tmp" && mv "$DESCRIPTOR.tmp" "$DESCRIPTOR"
fi
MSYS_NO_PATHCONV=1 docker rm -f "$DB_CONTAINER" >/dev/null 2>&1 || true
rm -f ".ai/qa/test-env.started"
echo "TEST_ENV_STATUS=stopped"
