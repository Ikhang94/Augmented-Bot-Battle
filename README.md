# Augmented Bot Battle

A multiplayer augmented reality game combining a Unity AR client with an ASP.NET Core backend. Players select robots that fight each other in AR, with match history, leaderboards, and player profiles managed through a RESTful API backed by PostgreSQL.

---

## Table of Contents

- [Architecture Overview](#architecture-overview)
- [Technology Stack](#technology-stack)
- [Project Structure](#project-structure)
- [Getting Started](#getting-started)
  - [Prerequisites](#prerequisites)
  - [Environment Setup](#environment-setup)
  - [Running the Backend](#running-the-backend)
  - [Running Tests](#running-tests)
  - [Running the Unity Client](#running-the-unity-client)
- [API Reference](#api-reference)
- [CI/CD Pipeline](#cicd-pipeline)
- [DevSecOps Tooling](#devsecops-tooling)
- [Taskfile Commands](#taskfile-commands)
- [Contributing](#contributing)

---

## Architecture Overview

```
┌─────────────────────────────────────┐
│         Unity AR Client             │
│  (ARFoundation / OpenXR / Android)  │
└──────────────┬──────────────────────┘
               │ HTTP REST
               ▼
┌─────────────────────────────────────┐
│       ASP.NET Core 9.0 API          │
│   Players · Robots · GameSessions   │
└──────────────┬──────────────────────┘
               │ EF Core / Npgsql
               ▼
┌─────────────────────────────────────┐
│           PostgreSQL                │
│  Players · Robots · GameSessions    │
│           Leaderboards              │
└─────────────────────────────────────┘
```

The Unity client handles all AR rendering and game logic. The backend API manages persistent data (players, robots, game sessions, leaderboards) and is deployed as a Docker container on Azure Container Apps.

---

## Technology Stack

| Layer | Technology |
|---|---|
| AR Client | Unity 6000.0.53f1, ARFoundation, OpenXR, Meta Quest |
| Backend API | ASP.NET Core 9.0, Entity Framework Core 9.0 |
| Database | PostgreSQL (via Npgsql 9.0) |
| Testing | XUnit 2.9.3, Moq 4.20.72, Coverlet, FluentAssertions |
| Containerization | Docker multi-stage, Azure Container Registry |
| Deployment | Azure Container Apps |
| CI/CD | GitLab CI, Taskfile v3 |
| Code Quality | MegaLinter, SonarQube, CSharpier, Roslynator |
| Security Scanning | Gitleaks, TruffleHog |

---

## Project Structure

```
.
├── project/
│   ├── Backend/
│   │   ├── BackendESP/                   # ASP.NET Core API (net9.0)
│   │   │   ├── Controllers/              # PlayerController, RobotController
│   │   │   ├── Data/                     # Repository pattern (IPlayerData, IRobotData)
│   │   │   ├── DTO/                      # PlayerDTO, RobotDTO
│   │   │   ├── Models/                   # EF Core entities + PostgresDbContext
│   │   │   └── Program.cs                # App entrypoint, DI setup, route mapping
│   │   └── BackendEsp.Tests/             # Unit tests (net10.0)
│   │       ├── Controllers/              # Controller tests with Moq
│   │       └── Data/                     # Repository tests
│   └── Unity/
│       └── ABB/                          # Unity AR project
│           ├── Assets/                   # Game assets, scripts, scenes
│           ├── Packages/                 # Package manifest
│           └── ProjectSettings/
├── .config/
│   ├── devsecops/                        # Taskfile orchestration per pipeline stage
│   │   ├── code/                         # Linters, commitlint, sonarqube tasks
│   │   ├── build/                        # Unity + Docker build tasks
│   │   ├── test/backend/                 # Test runner tasks
│   │   ├── release/                      # Versioning + ACR tagging tasks
│   │   └── deploy/                       # Azure deployment tasks
│   ├── gitlab/ci/                        # GitLab CI job definitions
│   │   └── devsecops/                    # One file per stage (code, build, test...)
│   ├── linters/                          # Linter configurations
│   │   ├── commitlint/config.yml
│   │   ├── cspell/.cspell.json
│   │   ├── csharpier/.csharpierrc.json
│   │   └── roslynator/.roslynatorconfig
│   └── docker/
│       ├── Dockerfile.prod
│       └── Dockerfile.dev
├── reports/coverage/                     # Test & coverage output (generated, gitignored)
├── VERSION                               # Current semantic version (auto-incremented)
└── taskfile.yml                          # Root Taskfile — entrypoint for all tasks
```

---

## Getting Started

### Prerequisites

| Tool | Version | Purpose |
|---|---|---|
| .NET SDK | 10.0+ | Backend build & tests |
| Unity Editor | 6000.0.53f1 | AR client |
| Docker | 24+ | Container builds |
| Task (go-task) | 3.50+ | Task orchestration |
| Bun | 1.3+ | JS tooling (commitlint) |
| PostgreSQL | 15+ | Local database |

Install Task:
```bash
curl -sL https://taskfile.dev/install.sh | sh
```

Install Bun:
```bash
curl -fsSL https://bun.sh/install | bash
```

### Environment Setup

Copy the example environment file and fill in your values:

```bash
cp .config/dotenv/.env.example .env
```

Required environment variables:

```dotenv
# Database
POSTGRES_HOST=localhost
POSTGRES_DB=augmentedbotbattle
POSTGRES_USER=postgres
POSTGRES_PASSWORD=yourpassword

# Unity (required for CI builds)
UNITY_VERSION=6000.0.53f1
UNITY_USERNAME=your@email.com
UNITY_PASSWORD=yourpassword
UNITY_SERIAL=YOUR-SERIAL-KEY

# SonarQube (required for CI analysis)
SONAR_PROJECT_KEY=your_project_key
SONAR_HOST_URL=https://your.sonar.instance
SONAR_TOKEN=your_token

# Azure (required for deployments)
SPN_CLIENT_ID=...
SPN_CLIENT_SECRET=...
SPN_TENANT_ID=...
AZ_SUBSCRIPTION_ID=...
AZURE_REGISTRY=yourregistry.azurecr.io
```

### Running the Backend

```bash
cd project/Backend/BackendESP
dotnet run
```

The API will be available at `http://localhost:5000`.

Or with Docker:

```bash
docker build -f .config/docker/Dockerfile.prod -t augmentedbotbattle .
docker run -p 80:80 --env-file .env augmentedbotbattle
```

### Running Tests

Run unit tests with coverage (CI mode — enforces 90% line coverage threshold):
```bash
task test
```

Run without coverage threshold (local development):
```bash
task test:dev
```

Coverage reports are generated in `reports/coverage/`:
- `test-results.xml` — JUnit format (GitLab CI test widget)
- `coverage.cobertura.xml` — Cobertura format (SonarQube + GitLab CI coverage widget)

### Running the Unity Client

1. Open Unity Hub and add the project at `project/Unity/ABB/`
2. Select Unity version **6000.0.53f1**
3. Open the main scene and press **Play** in the editor

For AR testing on Android:
- Enable Developer Mode on your device
- Connect via USB and use **Build & Run** targeting Android

---

## API Reference

All endpoints are prefixed with `/api`.

### Players — `/api/users`

| Method | Endpoint | Description |
|---|---|---|
| `POST` | `/api/users` | Create a new player |
| `GET` | `/api/users/getall` | Get all players |
| `GET` | `/api/users/{playerID}` | Get player by ID |
| `PUT` | `/api/users` | Update player |
| `DELETE` | `/api/users/{playerID}` | Delete player |

### Robots — `/api/robots`

| Method | Endpoint | Description |
|---|---|---|
| `GET` | `/api/robots` | Get all robots |
| `POST` | `/api/robots` | Create a new robot |
| `GET` | `/api/robots/{robotID}` | Get robot by ID |
| `PUT` | `/api/robots` | Update robot |
| `DELETE` | `/api/robots/{robotID}` | Delete robot |

### Data Models

**Player**
```json
{
  "playerId": 1,
  "username": "player1",
  "email": "player1@example.com",
  "password": "hashed_value"
}
```

**Robot**
```json
{
  "robotId": 1,
  "name": "RoninBot"
}
```

**GameSession**
```json
{
  "id": 1,
  "hostPlayerId": 1,
  "clientPlayerId": 2,
  "hostRobotId": 1,
  "clientRobotId": 2,
  "startTime": "2025-01-01T10:00:00",
  "endTime": "2025-01-01T10:15:00"
}
```

**Leaderboard**
```json
{
  "leaderboardId": 1,
  "playerId": 1,
  "rank": 3
}
```

---

## CI/CD Pipeline

The pipeline runs on GitLab CI and follows a 9-stage DevSecOps lifecycle.

### Trigger Matrix

| Event | Stages executed |
|---|---|
| Push on feature branch | `code` (lint, commitlint, megalinter) |
| Merge Request → `main` | `code` + `build` + `version:bump` |
| Push on `main` | `build` + `code:sonarqube` |
| Tag `production` | `release` + `deploy` |

### Pipeline Stages

```
plan → code → build → test → release → deploy → operate → monitor → feedback
```

**`code`** — Code quality gates
- `code:commitlint` — Validates commit messages against Conventional Commits spec
- `code:megalinter` — Runs YAML, C#, spell check, and secret scanners
- `code:sonarqube` — .NET static analysis with code coverage reporting (push to main only)

**`build`** — Compilation & packaging
- `build:unity` — Compiles Unity project for Android (caches `Library/` folder across runs)
- `build:all` — Multi-stage Docker build → pushes to Azure Container Registry

**`test`** — Automated testing
- `test` — XUnit unit tests with 90% coverage gate; produces JUnit + Cobertura reports

**`release`** — Versioning & image tagging
- `version:bump` — Auto-increments patch version in `VERSION` file on MR to main (`[skip ci]`)
- `release` — Tags Docker images in ACR with semantic version + `latest`

**`deploy`** — Azure deployment
- `deploy` — Deploys to staging environment (Azure Container Apps)
- `deploy:prod` — Deploys to production (triggered only by `production` tag)

---

## DevSecOps Tooling

### Code Quality — MegaLinter

Config: [`.config/devsecops/code/megalinter/config.yml`](.config/devsecops/code/megalinter/config.yml)

| Linter | Scope |
|---|---|
| `YAML_YAMLLINT` | YAML validation |
| `YAML_PRETTIER` | YAML formatting |
| `SPELL_CSPELL` | Spell checking (English + French) |
| `CSHARP_DOTNET_FORMAT` | .NET code formatting |
| `CSHARP_CSHARPIER` | C# opinionated formatting |
| `CSHARP_ROSLYNATOR` | Roslyn static analysis |
| `REPOSITORY_GITLEAKS` | Secret detection in git history |
| `REPOSITORY_TRUFFLEHOG` | Enhanced secret scanning |

### Static Analysis — SonarQube

SonarQube runs on every push to `main` and ingests Cobertura + OpenCover XML reports from the test job. Tracks line coverage, bugs, vulnerabilities, and code smells.

### Commit Convention — commitlint

Config: [`.config/linters/commitlint/config.yml`](.config/linters/commitlint/config.yml)

Commits must follow [Conventional Commits](https://www.conventionalcommits.org/):

```
<type>(<optional scope>): <description>

[optional body]
```

**Allowed types:** `build` `chore` `ci` `docs` `feat` `fix` `perf` `refactor` `revert` `style` `test`

Valid examples:
```
feat(player): add dodge mechanic
fix(api): handle null robot ID in controller
ci: add unity library cache to build job
docs: update API reference in README
```

> No space before the colon. `feat:` ✅ — `feat :` ❌

The `commit-msg` git hook validates each commit locally using the same config.

### Test Coverage — Coverlet

- **Tool:** `coverlet.msbuild` integrated into the test project
- **Threshold:** 90% line coverage (enforced in CI, not in `task test:dev`)
- **Exclusions:** `Program.cs`
- **Output formats:** Cobertura XML (GitLab CI + SonarQube), JUnit XML (GitLab test widget)

---

## Taskfile Commands

All commands are run from the repository root using [Task](https://taskfile.dev).

### Development

```bash
task test            # Run backend unit tests with 90% coverage gate
task test:dev        # Run backend unit tests without coverage threshold
```

### Code Quality

```bash
task code:lint       # Run all linters (commitlint + megalinter)
task code:fix        # Run auto-fixers
task code:sonarqube  # Run SonarQube analysis
task code:commitlint # Check commit messages only
ta
```

### Build

```bash
task build:backend   # Build .NET backend
task build:unity     # Build Unity project
task build:docker    # Build Docker image
```

### Release & Deploy

```bash
task version:bump    # Increment patch version in VERSION file
task release         # Tag and push Docker image to ACR
task deploy          # Deploy to staging (Azure Container Apps)
task deploy:prod     # Deploy to production
```

### Full Pipeline (local simulation)

```bash
task devsecops       # Run all stages sequentially: plan → feedback
```

---

## Contributing

1. **Branch naming:** `<type>/<short-description>` — e.g., `feat/add-leaderboard-endpoint`, `fix/null-robot-id`
2. **Commit format:** Follow Conventional Commits — enforced by the `commit-msg` hook on every commit
3. **Tests:** New features must maintain ≥ 90% line coverage on the `BackendESP` module
4. **Code style:** CSharpier and dotnet-format are enforced automatically by MegaLinter on CI

Before opening a Merge Request, run the full local check:

```bash
git fetch origin main   # Required for commitlint range detection
task code:lint
task test:dev
```