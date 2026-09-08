# Deployment Configuration Fixes

## Summary
Updated the deploy:dev task in `.config/devsecops/deploy/taskfile.yml` to properly configure environment variables for the OVH VPS backend deployment.

## Changes Made

### 1. Environment Variable Mapping (Lines 155-160)
**Problem:** Backend container was crashing with `JWT_SECRET environment variable is required`

**Solution:** 
- Added explicit export of all required environment variables in SSH script
- Mapped `JWT_SECRET_KEY` (from GitLab CI) to `JWT_SECRET` (expected by backend)
- Added defaults for POSTGRES_DB and POSTGRES_USER

```bash
export POSTGRES_DB="${POSTGRES_DB:-testdb}"
export POSTGRES_USER="${POSTGRES_USER:-testuser}"
export POSTGRES_PASSWORD="$POSTGRES_PASSWORD"
export JWT_SECRET="$JWT_SECRET_KEY"  # ← Key fix: renamed to JWT_SECRET
export JwtSettings__Issuer="${JWT_ISSUER:-TestIssuer}"
export JwtSettings__Audience="${JWT_AUDIENCE:-TestAudience}"
```

### 2. Dynamic docker-compose.yml Generation (Lines 181-198)
**Problem:** docker-compose.yml on OVH server didn't have environment variable configuration, causing container startup failures

**Solution:** 
- Deploy script now creates/updates docker-compose.yml on the server before pulling image
- Proper environment variable substitution using `${VAR_NAME}` syntax
- Uses Docker Compose v2 (no deprecated `version:` attribute)

```yaml
services:
  backend:
    image: ${FULL_IMAGE}
    restart: unless-stopped
    environment:
      POSTGRES_DB: ${POSTGRES_DB}
      POSTGRES_USER: ${POSTGRES_USER}
      POSTGRES_PASSWORD: ${POSTGRES_PASSWORD}
      JWT_SECRET: ${JWT_SECRET}
      JwtSettings__Issuer: ${JwtSettings__Issuer}
      JwtSettings__Audience: ${JwtSettings__Audience}
      ASPNETCORE_ENVIRONMENT: Production
      ASPNETCORE_URLS: http://+:80
    ports:
      - "80:80"
```

### 3. Variable Flow
1. GitLab CI loads variables from `.config/gitlab/ci/variables.yml`
2. Protected variables (POSTGRES_PASSWORD, JWT_SECRET_KEY, etc.) are set in GitLab Settings > CI/CD > Variables
3. Deploy task exports these as environment variables in SSH session
4. docker-compose.yml is created with variable references
5. When `docker compose up` runs, docker-compose substitutes variables with their values
6. Backend container starts with proper environment configuration

## Required GitLab CI Variables (Protected/Masked)
These must be configured in GitLab Settings > CI/CD > Variables:

| Variable | Required | Purpose |
|----------|----------|---------|
| POSTGRES_PASSWORD | ✅ | PostgreSQL database password |
| JWT_SECRET_KEY | ✅ | JWT token signing secret |
| JWT_ISSUER | ❌ | JWT issuer (defaults to "TestIssuer") |
| JWT_AUDIENCE | ❌ | JWT audience (defaults to "TestAudience") |
| AZURE_REGISTRY_USERNAME | ✅ | ACR login username |
| AZURE_REGISTRY_PASSWORD | ✅ | ACR login password |
| AZURE_REGISTRY_NAME | ✅ | ACR registry name (without .azurecr.io) |
| DEV_IP_ADDRESS | ✅ | OVH VPS IP address or hostname |
| SSH_PRIVATE_KEY | ✅ | SSH key (base64-encoded: `cat ~/.ssh/vps_key \| base64 -w0`) |

## Testing the Fix
1. Create a new deployment tag or trigger the deploy:dev job
2. The script will create/update docker-compose.yml on the OVH server
3. Backend container should start without JWT_SECRET or POSTGRES_PASSWORD errors
4. Container should connect to PostgreSQL successfully
5. Service should be accessible at http://[DEV_IP_ADDRESS]

## Troubleshooting
If container still crashes:
1. Check variable values in GitLab CI Settings > Variables
2. SSH to OVH server and verify environment variables:
   ```bash
   docker compose config  # Shows final computed configuration
   ```
3. Check container logs:
   ```bash
   docker compose logs backend
   ```
