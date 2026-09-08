# How to Use Linters

This document explains the linting and code quality tools used in this project, how to run them locally, how to fix errors, and where each configuration file lives.

---

## Table of Contents

- [Overview](#overview)
- [Running Linters Locally](#running-linters-locally)
- [Linters Reference](#linters-reference)
  - [CSpell — Spell Checking](#cspell--spell-checking)
  - [YAML — yamllint + Prettier](#yaml--yamllint--prettier)
  - [C# — CSharpier](#c--csharpier)
  - [C# — dotnet-format](#c--dotnet-format)
  - [C# — Roslynator](#c--roslynator)
  - [Secrets — Gitleaks + TruffleHog](#secrets--gitleaks--trufflehog)
  - [Commits — commitlint](#commits--commitlint)
- [Adding Words to CSpell](#adding-words-to-cspell)
- [Configuration Files Reference](#configuration-files-reference)

---

## Overview

Code quality is enforced by **MegaLinter**, which orchestrates all linters in a single Docker container during CI. Only the following linters are active (whitelist approach):

| Linter | Language/Scope | Auto-fix |
|---|---|---|
| `SPELL_CSPELL` | Spell checking (all files) | No |
| `YAML_YAMLLINT` | YAML validation | No |
| `YAML_PRETTIER` | YAML formatting | Yes |
| `CSHARP_DOTNET_FORMAT` | .NET code formatting | Yes |
| `CSHARP_CSHARPIER` | C# opinionated formatting | Yes |
| `CSHARP_ROSLYNATOR` | C# static analysis | Partial |
| `REPOSITORY_GITLEAKS` | Secret detection (git history) | No |
| `REPOSITORY_TRUFFLEHOG` | Secret detection (enhanced) | No |

Commit messages are validated separately by **commitlint** (not part of MegaLinter).

---

## Running Linters Locally

### Run all linters (via Taskfile)

```bash
task code:lint
```

This runs commitlint + MegaLinter in sequence.

### Auto-fix what can be fixed

```bash
task code:fix
```

CSharpier, dotnet-format, and Prettier can fix issues automatically.

### Run MegaLinter only (without Docker)

If you don't have Docker, MegaLinter can run without it using the `npx` fallback:

```bash
task code:megalinter
```

### Run commitlint only

```bash
task code:commitlint
```

> Requires `origin/main` to be up to date locally. Run `git fetch origin main` first if needed.

---

## Linters Reference

### CSpell — Spell Checking

**Config:** [`.config/linters/cspell/.cspell.json`](../../.config/linters/cspell/.cspell.json)

CSpell checks spelling in all source files. It supports English and French (`"language": "en,fr"`).

**Excluded from analysis:**
- `node_modules`, `.git`, `.config/linters/`
- `**/Unity/ABB/Assets/**` (Unity auto-generated assets)
- `*.csproj`, lock files, coverage reports

**When you get a CSpell error:**

If the word is a legitimate technical term, abbreviation, or project-specific word, add it to the `words` array in `.config/linters/cspell/.cspell.json`:

```json
"words": [
  "yourword"
]
```

See [Adding Words to CSpell](#adding-words-to-cspell) for guidelines.

---

### YAML — yamllint + Prettier

**Config:** [`.config/linters/yaml/.yamllint`](../../.config/linters/yaml/.yamllint)

yamllint checks YAML structure and style. Active rules:

| Rule | Level |
|---|---|
| Trailing spaces | error |
| Line length | error |
| Indentation | error |
| Duplicate keys | error |
| Document start (`---`) | warning |
| Comments indentation | warning |
| Truthy values | warning |

Prettier enforces consistent YAML formatting (spacing, quoting).

**Fix YAML formatting automatically:**

```bash
bunx --bun prettier --write "**/*.yml" "**/*.yaml"
```

---

### C# — CSharpier

**Config:** [`.config/linters/csharpier/.csharpierrc.json`](../../.config/linters/csharpier/.csharpierrc.json)

CSharpier is an opinionated C# formatter (similar to Prettier for C#). It enforces consistent style across all `.cs` files.

Current settings:
```json
{
  "printWidth": 100,
  "useTabs": true,
  "indentSize": 4,
  "endOfLine": "auto"
}
```

**Fix CSharpier issues automatically:**

```bash
dotnet csharpier .
```

Or through the Taskfile:

```bash
task code:fix
```

> CSharpier only applies to C# files. It does **not** rewrite logic — only formatting.

---

### C# — dotnet-format

dotnet-format enforces the .NET SDK formatting rules (whitespace, encoding, style analyzers). It respects `.editorconfig` settings.

**Fix dotnet-format issues automatically:**

```bash
dotnet format project/Backend/BackendESP/BackendESP.csproj
```

---

### C# — Roslynator

**Config:** [`.config/linters/roslynator/.roslynatorconfig`](../../.config/linters/roslynator/.roslynatorconfig)

Roslynator runs Roslyn-based C# analyzers. It catches code smells, unused variables, redundant casts, and other issues that the compiler warnings miss.

Roslynator rules only apply to Unity C# files (`Unity/ABB/Assets/**/*.cs`).

**Common Roslynator findings:**
- Unused `using` directives
- Redundant `null` checks
- Unnecessary `else` after `return`
- Variables that can be made `readonly`

Some findings can be auto-fixed:

```bash
dotnet roslynator fix project/Backend/BackendESP/BackendESP.csproj
```

---

### Secrets — Gitleaks + TruffleHog

Gitleaks and TruffleHog scan for accidentally committed secrets (API keys, passwords, tokens).

**Gitleaks** scans the last 10 commits (`HEAD~10..HEAD`).  
**TruffleHog** performs a deeper scan of the repository content.

**If you get a secret detection error:**

1. Identify the secret in the reported file and line
2. Remove it from the file immediately
3. If it was already committed, rotate the secret (invalidate the old key)
4. Rewrite git history to remove the commit:
   ```bash
   git rebase -i HEAD~<n>   # mark the offending commit as 'drop' or 'edit'
   git push --force-with-lease origin HEAD
   ```
5. Add the pattern to `.gitleaks.toml` only if it is a **false positive** (e.g., a test fixture value)

> Never commit real credentials, tokens, or passwords. Use environment variables loaded from `.env` files (which are gitignored).

---

### Commits — commitlint

**Config:** [`.config/linters/commitlint/config.yml`](../../.config/linters/commitlint/config.yml)

commitlint enforces the [Conventional Commits](https://www.conventionalcommits.org/) specification on every commit message.

**Format:**

```
<type>(<optional scope>): <description>

[optional body — separated by a blank line]
```

**Allowed types:**

| Type | When to use |
|---|---|
| `feat` | A new feature |
| `fix` | A bug fix |
| `build` | Build system or dependency changes |
| `ci` | CI/CD configuration changes |
| `docs` | Documentation only |
| `refactor` | Code restructuring without behavior change |
| `test` | Adding or updating tests |
| `style` | Formatting, no logic changes |
| `perf` | Performance improvements |
| `chore` | Maintenance tasks |
| `revert` | Reverting a previous commit |

**Valid examples:**

```
feat(player): add dodge mechanic
fix(api): handle null robot ID in controller
ci: add unity library cache to build job
docs: update API reference
build: bump version 0.1.4 → 0.1.5 [skip ci]
```

**Common mistakes:**

```
# Space before colon — INVALID
feat : add something

# Missing type — INVALID
add something

# Non-standard type — INVALID
bump: new version
update: something
```

**Fix a commit message:**

```bash
# Fix the last commit message
git commit --amend

# Fix multiple commits on your branch
git rebase -i $(git merge-base HEAD origin/main)
# Change 'pick' to 'reword' for commits to fix, save, then edit each message
```

The `commit-msg` git hook runs commitlint automatically on every `git commit`. If the hook is not installed, run:

```bash
task install   # or equivalent hook installation task
```

---

## Adding Words to CSpell

Before adding a word, verify it is genuinely needed:

- Is it a proper technical term? (e.g., `Npgsql`, `ARFoundation`)
- Is it a project-specific name? (e.g., `RoninBot`, `Gamesession`)
- Is it a Unity or .NET API name? (e.g., `Mathf`, `MonoBehaviour`)

If yes, add it to the `words` array in [`.config/linters/cspell/.cspell.json`](../../.config/linters/cspell/.cspell.json):

```json
"words": [
  "existingword",
  "yournewword"
]
```


**Guidelines:**
- Use lowercase in the `words` array — CSpell matches case-insensitively
- Do not add common English words that are simply misspelled — fix the typo instead
- Do not add entire file paths — use `ignorePaths` for that
- If a whole directory should be excluded (e.g., auto-generated code), add it to `ignorePaths`:

```json
"ignorePaths": [
  "**/Unity/ABB/Assets/**"
]
```

---

## Configuration Files Reference

| Tool | Config file |
|---|---|
| MegaLinter | `.config/devsecops/code/megalinter/config.yml` |
| CSpell | `.config/linters/cspell/.cspell.json` |
| yamllint | `.config/linters/yaml/.yamllint` |
| CSharpier | `.config/linters/csharpier/.csharpierrc.json` |
| Roslynator | `.config/linters/roslynator/.roslynatorconfig` |
| commitlint | `.config/linters/commitlint/config.yml` |
