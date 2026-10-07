#!/bin/sh
# om-prepare-test-env: generated entrypoint (contract v2)
# regenerate with: om-prepare-test-env --regenerate
# history:
#   2026-10-07 generated (cold 82s incl. Docker Desktop start, warm 6s reused, source change 62s rebuild): ephemeral Postgres 16 container + Home.Scout.Web run from its build output
#   2026-10-07 repair: start Docker Desktop and wait when the daemon is down (first cold run failed with "docker is not running")
set -eu

# ---------------------------------------------------------------------------
# Project-specific variables (discovered from docker-compose.yml, Dockerfile,
# AGENTS.md and the Web project). Tweak here; the logic below stays generic.
# ---------------------------------------------------------------------------
WEB_PROJECT="src/Home.Scout.Web/Home.Scout.Web.csproj"
WEB_DIR="src/Home.Scout.Web"
WEB_DLL="bin/Debug/net9.0/Home.Scout.Web.dll"        # relative to WEB_DIR
APP_PREFERRED_PORT="${TEST_ENV_APP_PORT:-5299}"
HEALTH_PATH="/News"                                   # 200 only when the app can query the database
DB_IMAGE="postgres:16-alpine"                         # matches docker-compose.yml
DB_PREFERRED_PORT="${TEST_ENV_DB_PORT:-15432}"
DB_NAME="HomeScout"
DB_USER="homescout"
DB_PASSWORD="qa-local-only"                           # disposable demo value, never a real secret
DB_INIT_DIR="src/Home.Scout.Infrastructure/Data/Scripts"   # mounted into docker-entrypoint-initdb.d like compose does
DB_READY_TABLE='"RssChannels"'
BUILD_INPUTS="src Home.Scout.sln"
BUILD_ENV_VARS="ASPNETCORE_ENVIRONMENT"
ARTIFACTS="$WEB_DIR/$WEB_DLL"
TTL="${TEST_ENV_CACHE_TTL_SECONDS:-600}"
# The project targets net9.0; machines with only a newer runtime still run it.
# Major roll-forward is a no-op where the .NET 9 runtime is installed.
export DOTNET_ROLL_FORWARD=Major
export ASPNETCORE_ENVIRONMENT=Development

# ---------------------------------------------------------------------------
FORCE=0; FORCE_REBUILD=0
for arg in "$@"; do
  case "$arg" in
    --force) FORCE=1 ;;
    --force-rebuild) FORCE_REBUILD=1 ;;
    *) echo "unknown flag: $arg" >&2; exit 2 ;;
  esac
done

ROOT=$(cd "$(dirname "$0")/../.." && pwd)
cd "$ROOT"
QA_DIR=".ai/qa"
DESCRIPTOR="$QA_DIR/test-env.json"
BUILD_CACHE="$QA_DIR/test-env-build-cache.json"
LOCK_DIR="$QA_DIR/test-env.lock"
STARTED_MARKER="$QA_DIR/test-env.started"
APP_LOG="$QA_DIR/test-env-app.log"
mkdir -p "$QA_DIR"

# Scope container names to this checkout, so worktrees get their own database.
SCOPE=$(printf '%s' "$ROOT" | cksum | awk '{print $1}')
DB_CONTAINER="home-scout-qa-db-$SCOPE"

case "$(uname -s 2>/dev/null || echo unknown)" in
  MINGW*|MSYS*|CYGWIN*) PLATFORM=win32 ;;
  Darwin*) PLATFORM=darwin ;;
  Linux*) if grep -qiE 'microsoft|wsl' /proc/version 2>/dev/null; then PLATFORM=wsl2; else PLATFORM=linux; fi ;;
  *) PLATFORM=unknown ;;
esac

now() { date -u +%Y-%m-%dT%H:%M:%SZ; }
epoch() { date -u +%s; }
log() { printf '[test-env] %s\n' "$*" >&2; }
json_get() { sed -n "s/.*\"$1\": *\"\{0,1\}\([^\",}]*\)\"\{0,1\}.*/\1/p" "$2" 2>/dev/null | head -n 1; }

host_path() {   # docker on Git Bash needs a Windows path for bind mounts
  if [ "$PLATFORM" = win32 ] && command -v cygpath >/dev/null 2>&1; then cygpath -w "$1"; else printf '%s' "$1"; fi
}

free_port() {
  if command -v python3 >/dev/null 2>&1; then
    python3 -c 'import socket;s=socket.socket();s.bind(("127.0.0.1",0));print(s.getsockname()[1]);s.close()'
  elif command -v node >/dev/null 2>&1; then
    node -e 's=require("net").createServer();s.listen(0,"127.0.0.1",()=>{console.log(s.address().port);s.close()})'
  else
    awk 'BEGIN{srand();print 20000+int(rand()*20000)}'
  fi
}
random_port() { awk -v s="$$$(date +%s)" 'BEGIN{srand(s);print 20000+int(rand()*20000)}'; }
port_busy() { curl -s -o /dev/null --max-time 1 "http://127.0.0.1:$1/" 2>/dev/null; }

pid_alive() {
  [ -n "${1:-}" ] || return 1
  if [ "$PLATFORM" = win32 ] && [ -n "${2:-}" ]; then
    tasklist //FI "PID eq $2" 2>/dev/null | grep -q "$2"
  else
    kill -0 "$1" 2>/dev/null
  fi
}

# --- lock: one bootstrap at a time -----------------------------------------
acquire_lock() {
  waited=0
  while ! mkdir "$LOCK_DIR" 2>/dev/null; do
    owner=$(json_get pid "$LOCK_DIR/owner.json")
    if [ -z "$owner" ] || ! kill -0 "$owner" 2>/dev/null; then
      log "removing stale lock (owner ${owner:-unknown} is gone)"
      rm -rf "$LOCK_DIR"; continue
    fi
    [ "$waited" -ge 300 ] && { log "lock held by pid $owner for 5 min; giving up"; exit 1; }
    sleep 3; waited=$((waited + 3))
  done
  printf '{ "pid": %s, "source": "test-env-up.sh", "acquiredAt": "%s" }\n' "$$" "$(now)" > "$LOCK_DIR/owner.json"
}
release_lock() { rm -rf "$LOCK_DIR"; }
acquire_lock
trap release_lock EXIT INT TERM

# --- browser provider (verified at generation; warm runs only re-detect) ----
detect_browser() {
  BROWSER_CMD=""
  if command -v agent-browser >/dev/null 2>&1; then BROWSER_CMD=$(command -v agent-browser)
  else
    for c in \
      "${LOCALAPPDATA:-}/agent-tools/agent-browser/v0.35.2/agent-browser-win32-x64.exe" \
      "${XDG_CACHE_HOME:-$HOME/.cache}/agent-tools/agent-browser/v0.35.2/"agent-browser-*; do
      [ -n "$c" ] && [ -f "$c" ] && { BROWSER_CMD="$c"; break; }
    done
  fi
  if [ -n "$BROWSER_CMD" ]; then
    [ "$PLATFORM" = win32 ] && command -v cygpath >/dev/null 2>&1 && BROWSER_CMD=$(cygpath -m "$BROWSER_CMD")
    BROWSER_INSTALLED=1
  else
    BROWSER_INSTALLED=0
  fi
}

print_result() {
  echo "TEST_ENV_STATUS=running"
  echo "TEST_ENV_BASE_URL=$1"
  echo "TEST_ENV_DESCRIPTOR=$DESCRIPTOR"
  echo "TEST_ENV_REUSED=$2"
  echo "BROWSER_PROVIDER=agent-browser"
  echo "BROWSER_INSTALLED=$BROWSER_INSTALLED"
}

# --- reuse check: attach, don't reboot -------------------------------------
stale_sources() {   # any tracked source newer than the running app's start
  [ -f "$STARTED_MARKER" ] || return 0
  [ -n "$(find src -type f -newer "$STARTED_MARKER" ! -path '*/bin/*' ! -path '*/obj/*' 2>/dev/null | head -n 1)" ]
}

teardown_previous() {
  if [ -f "$DESCRIPTOR" ]; then
    old_pid=$(json_get pid "$DESCRIPTOR"); old_win=$(json_get winPid "$DESCRIPTOR")
    if pid_alive "$old_pid" "$old_win"; then
      log "stopping previous app (pid $old_pid)"
      kill "$old_pid" 2>/dev/null || true
      [ "$PLATFORM" = win32 ] && [ -n "$old_win" ] && taskkill //PID "$old_win" //F >/dev/null 2>&1 || true
    fi
  fi
  MSYS_NO_PATHCONV=1 docker rm -f "$DB_CONTAINER" >/dev/null 2>&1 || true
}

detect_browser
if [ "$FORCE" = 0 ] && [ -f "$DESCRIPTOR" ] && [ "$(json_get status "$DESCRIPTOR")" = running ]; then
  base=$(json_get baseUrl "$DESCRIPTOR"); pid=$(json_get pid "$DESCRIPTOR"); win=$(json_get winPid "$DESCRIPTOR")
  started=$(json_get startedEpoch "$DESCRIPTOR")
  age=$(( $(epoch) - ${started:-0} ))
  if pid_alive "$pid" "$win" \
     && curl -fsS -o /dev/null --max-time 5 "$base/" -L \
     && curl -fsS -o /dev/null --max-time 10 "$base$HEALTH_PATH" \
     && [ "$age" -le "$TTL" ] && ! stale_sources; then
    log "reusing running environment (age ${age}s)"
    print_result "$base" 1
    exit 0
  fi
  log "previous environment is stale or unhealthy; restarting"
fi
teardown_previous

# --- build cache ------------------------------------------------------------
fp_file() { stat -c '%s:%Y' "$1" 2>/dev/null || stat -f '%z:%m' "$1" 2>/dev/null; }
fingerprint() {
  {
    for p in $BUILD_INPUTS; do
      if [ -d "$p" ]; then
        find "$p" -type f ! -path '*/bin/*' ! -path '*/obj/*' ! -path '*/.git/*' ! -path '*/node_modules/*'
      elif [ -f "$p" ]; then echo "$p"; fi
    done | LC_ALL=C sort | while IFS= read -r f; do printf '%s:%s\n' "$f" "$(fp_file "$f")"; done
    for v in $BUILD_ENV_VARS; do eval "printf 'env:%s=%s\n' \"$v\" \"\${$v:-}\""; done
  } | cksum | awk '{print $1"-"$2}'
}
build_needed() {
  [ "$FORCE_REBUILD" = 1 ] && return 0
  [ -f "$BUILD_CACHE" ] || return 0
  [ "$(json_get sourceFingerprint "$BUILD_CACHE")" = "$(fingerprint)" ] || return 0
  [ "$(json_get projectRoot "$BUILD_CACHE")" = "$ROOT" ] || return 0
  for a in $ARTIFACTS; do [ -s "$a" ] || return 0; done
  return 1
}
BUILT=0
if build_needed; then
  log "building $WEB_PROJECT"
  dotnet build "$WEB_PROJECT" -c Debug -nologo -v q >"$QA_DIR/test-env-build.log" 2>&1 \
    || { log "build failed; see $QA_DIR/test-env-build.log"; tail -n 20 "$QA_DIR/test-env-build.log" >&2; exit 1; }
  printf '{ "builtAt": "%s", "sourceFingerprint": "%s", "projectRoot": "%s", "artifactPaths": "%s" }\n' \
    "$(now)" "$(fingerprint)" "$ROOT" "$ARTIFACTS" > "$BUILD_CACHE"
  BUILT=1
else
  log "build cache valid; skipping build"
fi

# --- services up: disposable Postgres ----------------------------------------
if ! docker info >/dev/null 2>&1; then
  DD_WIN="/c/Program Files/Docker/Docker/Docker Desktop.exe"
  if [ "$PLATFORM" = win32 ] && [ -f "$DD_WIN" ]; then
    log "Docker is not running; starting Docker Desktop"
    ( "$DD_WIN" >/dev/null 2>&1 & )
  elif [ "$PLATFORM" = darwin ] && [ -d "/Applications/Docker.app" ]; then
    log "Docker is not running; starting Docker Desktop"
    open -a Docker
  else
    log "docker is not running; start Docker and retry"; exit 1
  fi
  waited=0
  until docker info >/dev/null 2>&1; do
    [ "$waited" -ge 180 ] && { log "Docker did not become ready within 180s"; exit 1; }
    sleep 3; waited=$((waited + 3))
  done
fi
DB_PORT="$DB_PREFERRED_PORT"
attempt=0
until MSYS_NO_PATHCONV=1 docker run -d --rm --name "$DB_CONTAINER" \
      -e POSTGRES_DB="$DB_NAME" -e POSTGRES_USER="$DB_USER" -e POSTGRES_PASSWORD="$DB_PASSWORD" \
      -p "127.0.0.1:$DB_PORT:5432" \
      -v "$(host_path "$ROOT/$DB_INIT_DIR"):/docker-entrypoint-initdb.d:ro" \
      "$DB_IMAGE" >/dev/null 2>"$QA_DIR/test-env-docker.err"; do
  attempt=$((attempt + 1))
  MSYS_NO_PATHCONV=1 docker rm -f "$DB_CONTAINER" >/dev/null 2>&1 || true
  [ "$attempt" -ge 5 ] && { log "could not start $DB_IMAGE"; cat "$QA_DIR/test-env-docker.err" >&2; exit 1; }
  DB_PORT=$(random_port)   # preferred port taken or reserved (Windows excludes port ranges)
  log "database port busy; retrying on $DB_PORT"
done
log "waiting for Postgres on 127.0.0.1:$DB_PORT"
waited=0
until MSYS_NO_PATHCONV=1 docker exec "$DB_CONTAINER" pg_isready -h 127.0.0.1 -U "$DB_USER" -d "$DB_NAME" >/dev/null 2>&1 \
   && [ "$(MSYS_NO_PATHCONV=1 docker exec "$DB_CONTAINER" psql -U "$DB_USER" -d "$DB_NAME" -tAc "select to_regclass('$DB_READY_TABLE') is not null" 2>/dev/null)" = t ]; do
  [ "$waited" -ge 90 ] && { log "Postgres not ready after 90s"; MSYS_NO_PATHCONV=1 docker logs "$DB_CONTAINER" 2>&1 | tail -n 20 >&2; exit 1; }
  sleep 2; waited=$((waited + 2))
done
CONN="Host=127.0.0.1;Port=$DB_PORT;Database=$DB_NAME;Username=$DB_USER;Password=$DB_PASSWORD"

# --- app start + health wait --------------------------------------------------
APP_PORT="$APP_PREFERRED_PORT"
port_busy "$APP_PORT" && APP_PORT=$(free_port)
BASE_URL="http://127.0.0.1:$APP_PORT"
touch "$STARTED_MARKER"
(
  cd "$WEB_DIR"
  ConnectionStrings__DefaultConnection="$CONN" ASPNETCORE_URLS="$BASE_URL" \
    nohup dotnet "$WEB_DLL" >"$ROOT/$APP_LOG" 2>&1 &
  echo $! > "$ROOT/$QA_DIR/test-env.pid"
)
APP_PID=$(cat "$QA_DIR/test-env.pid"); rm -f "$QA_DIR/test-env.pid"
WIN_PID=""
[ "$PLATFORM" = win32 ] && [ -r "/proc/$APP_PID/winpid" ] && WIN_PID=$(cat "/proc/$APP_PID/winpid")
log "waiting for $BASE_URL$HEALTH_PATH (pid $APP_PID)"
waited=0
until curl -fsS -o /dev/null --max-time 5 "$BASE_URL$HEALTH_PATH"; do
  if ! pid_alive "$APP_PID" "$WIN_PID"; then log "app exited during startup"; tail -n 30 "$APP_LOG" >&2; exit 1; fi
  [ "$waited" -ge 90 ] && { log "app not healthy after 90s"; tail -n 30 "$APP_LOG" >&2; exit 1; }
  sleep 1; waited=$((waited + 1))
done

# --- descriptor + result lines ------------------------------------------------
RUN_ID="$(date -u +%Y%m%d%H%M%S)-$SCOPE"
cat > "$DESCRIPTOR" <<EOF
{
  "version": 1,
  "runId": "$RUN_ID",
  "status": "running",
  "mode": "ephemeral",
  "baseUrl": "$BASE_URL",
  "startedByThisRepo": true,
  "startScript": ".ai/scripts/test-env-up.sh",
  "stopScript": ".ai/scripts/test-env-down.sh",
  "app": { "startCommand": "dotnet $WEB_DIR/$WEB_DLL", "port": $APP_PORT, "healthPath": "$HEALTH_PATH", "pid": $APP_PID, "winPid": "$WIN_PID", "log": "$APP_LOG" },
  "services": [
    { "type": "postgres", "host": "127.0.0.1", "port": $DB_PORT, "container": "$DB_CONTAINER", "env": { "ConnectionStrings__DefaultConnection": "$CONN" } }
  ],
  "credentials": [],
  "browser": {
    "provider": "agent-browser",
    "installed": $( [ "$BROWSER_INSTALLED" = 1 ] && echo true || echo false ),
    "command": "$BROWSER_CMD",
    "version": "0.35.2",
    "descriptor": ".ai/browsers/agent-browser.md",
    "notes": ""
  },
  "testRunner": { "name": "none", "config": "" },
  "platform": "$PLATFORM",
  "startedAt": "$(now)",
  "startedEpoch": $(epoch),
  "notes": "Database starts empty (schema only, from $DB_INIT_DIR); create channels through the UI. The background RSS worker is not started; use Fetch now. App runs with DOTNET_ROLL_FORWARD=Major. Verified timings: cold 82s (incl. Docker Desktop start), warm 6s (reused). built=$BUILT"
}
EOF
print_result "$BASE_URL" 0
