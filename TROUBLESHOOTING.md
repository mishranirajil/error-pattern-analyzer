# Error Pattern Analyzer - Troubleshooting Guide

## Common Issues & Solutions

### 1. Build Errors

#### "The project file was not found"
**Cause:** Missing project files

**Solution:**
```bash
# Verify all projects exist
dotnet sln ErrorPatternAnalyzer.sln list

# Should show:
# - src\ErrorPatternAnalyzer.Core\ErrorPatternAnalyzer.Core.csproj
# - src\ErrorPatternAnalyzer.Infrastructure\ErrorPatternAnalyzer.Infrastructure.csproj
# - src\ErrorPatternAnalyzer.Worker\ErrorPatternAnalyzer.Worker.csproj
# - src\ErrorPatternAnalyzer.Api\ErrorPatternAnalyzer.Api.csproj
# - tests\ErrorPatternAnalyzer.Tests\ErrorPatternAnalyzer.Tests.csproj
```

#### "Package 'X' version >= Y not found"
**Cause:** Package version not available

**Solution:**
```bash
# Clear NuGet cache
dotnet nuget locals all --clear

# Restore packages
dotnet restore ErrorPatternAnalyzer.sln
```

### 2. Runtime Errors

#### "New Relic connection failed"

**Possible Causes:**

1. **Missing Account ID**
   ```json
   "AccountId": ""  // ❌ Empty
   ```
   **Fix:** Add your Account ID from New Relic

2. **Invalid API Key**
   ```json
   "ApiKey": "invalid-key"  // ❌ Doesn't start with NRAK-
   ```
   **Fix:** API Key must start with `NRAK-`

3. **Wrong Key Type**
   - You need: **Insights Query Key**
   - Not: User API Key or License Key
   
   **Fix:** Create new key: New Relic → API Keys → Create Key → Type: "Insights Query Key"

4. **Insufficient Permissions**
   **Fix:** Verify your user account has "NRQL query" permissions

**Test Connection:**
```bash
# From Worker directory
dotnet run

# Should see:
# [INF] ✓ New Relic connection successful

# If you see:
# [WRN] ✗ New Relic connection failed
# Check the error details in logs
```

#### "No errors found for '{AppName}'"

**Possible Causes:**

1. **Application Name Mismatch**
   ```json
   "Name": "Registration-API"  // What you configured
   ```
   **vs.**
   ```
   Actual name in New Relic: "registration-api"  // Different case/format
   ```
   
   **Fix:** Check exact name in New Relic APM → Applications

2. **No Errors in Time Window**
   - Default lookback: 15 minutes
   - If no errors occurred recently, nothing to fetch
   
   **Fix:** 
   ```json
   "LookbackMinutes": 60  // Increase to 60 minutes
   ```

3. **Application Not Reporting**
   - Application might be offline
   - New Relic agent not configured
   
   **Fix:** Verify app is sending data to New Relic

#### "MongoDB connection failed"

**Possible Causes:**

1. **Docker Not Running**
   ```bash
   docker ps
   # If empty, Docker is not running or containers stopped
   ```
   
   **Fix:**
   ```bash
   # Start Docker Desktop
   # Then:
   docker-compose up -d
   ```

2. **Port 27017 Already in Use**
   ```
   Error: bind: address already in use
   ```
   
   **Fix:**
   ```bash
   # Find what's using port 27017
   netstat -ano | findstr :27017
   
   # Stop existing MongoDB:
   # Option 1: Stop the process
   # Option 2: Change port in docker-compose.yml
   ports:
     - "27018:27017"  # Use 27018 instead
   ```

3. **Connection String Wrong**
   ```json
   "ConnectionString": "mongodb://localhost:27017"  // ✅ Correct
   "ConnectionString": "mongodb://127.0.0.1:27017"  // ✅ Also works
   "ConnectionString": "mongodb://mongodb:27017"    // ❌ Wrong (use localhost)
   ```

### 3. Configuration Issues

#### Configuration Not Loading

**Check:**
1. File exists: `src/ErrorPatternAnalyzer.Worker/appsettings.json`
2. Valid JSON (use online JSON validator)
3. No trailing commas
4. Proper escaping of special characters

**Test:**
```bash
# Pretty print JSON to verify syntax
Get-Content appsettings.json | ConvertFrom-Json | ConvertTo-Json
```

#### "NewRelic AccountId is required"

**Cause:** Configuration not found or invalid

**Debug:**
```csharp
// Add to Program.cs to debug config
var config = builder.Configuration.GetSection("NewRelic").Get<NewRelicConfig>();
Console.WriteLine($"AccountId: {config?.AccountId}");
Console.WriteLine($"ApiKey: {config?.ApiKey?.Substring(0, 10)}...");
```

### 4. Logging Issues

#### No Logs Appearing

**Check:**
1. **Console Logs:** Should appear immediately
2. **File Logs:** Created in `logs/` subdirectory

**Verify Serilog Configuration:**
```csharp
// In Program.cs
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/worker-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

// Test logging
Log.Information("Test message");
```

#### Log Files Not Created

**Cause:** Permissions or path issues

**Fix:**
```bash
# Create logs directory manually
mkdir logs

# Or use absolute path in config
.WriteTo.File("D:/Logs/worker-.txt", ...)
```

### 5. New Relic Query Issues

#### "Query returned 0 results" but errors exist in New Relic

**Debug with NRQL:**
1. Go to New Relic → Query Your Data
2. Run this query:
   ```sql
   SELECT count(*) 
   FROM TransactionError 
   WHERE appName = 'YOUR_APP_NAME' 
   SINCE 1 hour ago
   ```
3. Verify results > 0

**If results are 0:**
- Check application name exact match
- Check time window (SINCE)
- Verify TransactionError events exist (not just Transaction)

#### Query Timeout

**Symptoms:**
```
HttpRequestException: The request timed out after 30 seconds
```

**Fix:**
```json
"NewRelic": {
  "QueryTimeout": 60,  // Increase from 30 to 60 seconds
  "MaxErrorsPerPoll": 5000  // Reduce if timing out
}
```

### 6. Docker Issues

#### "Unable to start container: mongo"

**Cause:** Docker service not running or port conflict

**Fix:**
```bash
# Check Docker is running
docker --version

# Check port availability
netstat -ano | findstr :27017

# Stop conflicting services
docker stop $(docker ps -aq)

# Restart Docker Compose
docker-compose down
docker-compose up -d
```

#### MongoDB Express Not Accessible

**Check:**
```bash
# Verify container is running
docker ps | findstr mongo-express

# Check logs
docker logs error-analyzer-mongo-express

# Access at: http://localhost:8081
# Credentials: admin / admin
```

## Validation Checklist

### ✅ Pre-Run Checklist

- [ ] .NET 8 SDK installed (`dotnet --version`)
- [ ] Docker Desktop running (`docker ps`)
- [ ] MongoDB container running (`docker ps | findstr mongo`)
- [ ] New Relic Account ID obtained
- [ ] New Relic Query API Key created (starts with NRAK-)
- [ ] Application name matches exactly in New Relic
- [ ] `appsettings.json` configured with credentials
- [ ] Solution builds successfully (`dotnet build`)
- [ ] Tests pass (`dotnet test`)

### ✅ Post-Start Checklist

- [ ] Worker starts without exceptions
- [ ] Logs show "✓ New Relic connection successful"
- [ ] Logs show "Querying New Relic for errors"
- [ ] Logs show "Fetched X errors" (X > 0 if errors exist)
- [ ] No error stack traces in console
- [ ] Log files created in `logs/` directory
- [ ] MongoDB accessible at http://localhost:8081

## Debug Mode

### Enable Verbose Logging

**In `appsettings.Development.json`:**
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Debug",
      "Microsoft": "Debug",
      "System": "Debug",
      "ErrorPatternAnalyzer": "Trace"
    }
  }
}
```

**Run with Development environment:**
```bash
$env:ASPNETCORE_ENVIRONMENT="Development"
dotnet run
```

### Test New Relic Connection Manually

**Create test file: `TestNewRelic.ps1`:**
```powershell
$accountId = "YOUR_ACCOUNT_ID"
$apiKey = "NRAK-YOUR_KEY"

$nrql = "SELECT count(*) FROM Transaction SINCE 1 minute ago LIMIT 1"
$url = "https://insights-api.newrelic.com/v1/accounts/$accountId/query?nrql=$([uri]::EscapeDataString($nrql))"

$headers = @{
    "X-Query-Key" = $apiKey
    "Accept" = "application/json"
}

try {
    $response = Invoke-RestMethod -Uri $url -Headers $headers -Method Get
    Write-Host "✓ Connection successful!" -ForegroundColor Green
    Write-Host ($response | ConvertTo-Json -Depth 3)
} catch {
    Write-Host "✗ Connection failed!" -ForegroundColor Red
    Write-Host $_.Exception.Message
}
```

**Run:**
```bash
.\TestNewRelic.ps1
```

## Getting Help

### Log Analysis

**Important log patterns to look for:**

1. **Success:**
   ```
   [INF] ✓ New Relic connection successful
   [INF] Fetched 1234 errors from 'Registration-API'
   [INF] Converted 1234 New Relic errors to ErrorEntry models
   ```

2. **Configuration Issues:**
   ```
   [ERR] NewRelic AccountId is required
   [ERR] NewRelic ApiKey must be a Query Key (starts with NRAK-)
   ```

3. **Connection Issues:**
   ```
   [ERR] New Relic API error: Unauthorized (401)
   [ERR] New Relic API error: Not Found (404)
   ```

4. **Query Issues:**
   ```
   [INF] No errors found for 'Registration-API'
   [WRN] Query returned 0 results
   ```

### Common Error Messages

| Error | Cause | Solution |
|-------|-------|----------|
| `401 Unauthorized` | Invalid API key | Verify key starts with NRAK- |
| `404 Not Found` | Invalid Account ID | Check Account ID in New Relic |
| `Timeout` | Query too slow | Reduce MaxErrorsPerPoll |
| `Connection refused` | MongoDB not running | Start Docker Compose |
| `Address already in use` | Port conflict | Change port in docker-compose.yml |

### Contact & Support

1. **Check logs:** `logs/worker-{date}.txt`
2. **Enable debug logging:** Set LogLevel to "Debug"
3. **Test New Relic:** Use PowerShell test script above
4. **Verify Docker:** `docker ps` should show MongoDB
5. **Check application name:** Must match exactly in New Relic

### Useful Commands

```bash
# View real-time logs
Get-Content logs/worker-20250122.txt -Wait

# Check Docker containers
docker ps -a

# MongoDB connection test
docker exec -it error-analyzer-mongodb mongosh --eval "db.adminCommand('ping')"

# View MongoDB databases
docker exec -it error-analyzer-mongodb mongosh --eval "show dbs"

# Restart everything
docker-compose down ; docker-compose up -d ; cd src/ErrorPatternAnalyzer.Worker ; dotnet run
```

---

**Last Updated:** 2025-01-22
**Version:** 1.0.0
**Status:** Phase 1 Complete
