# Error Pattern Analyzer - Quick Start Guide

## Prerequisites

- .NET 8 SDK
- Docker Desktop (for MongoDB)
- New Relic account with Query API access

## Step 1: Get New Relic Credentials

1. **Login to New Relic**: https://one.newrelic.com
2. **Get Account ID**:
   - Look at URL: `https://one.newrelic.com/accounts/{ACCOUNT_ID}/...`
   - Or go to Account Settings
3. **Create Query Key**:
   - Navigate to: User menu → API Keys
   - Click "Create a key"
   - Type: "Insights Query Key"
   - Name: "Error Pattern Analyzer"
   - Copy the key (starts with `NRAK-`)

## Step 2: Configure Application

Edit `src/ErrorPatternAnalyzer.Worker/appsettings.json`:

```json
{
  "NewRelic": {
    "AccountId": "1234567",                          // ← Your account ID
    "ApiKey": "NRAK-XXXXXXXXXXXXXXXXXXXXX",          // ← Your query key
    "Applications": [
      {
        "Name": "Registration-API",                   // ← Your app name in New Relic
        "Enabled": true,
        "Repository": "ImagineLearning/registration",
        "Team": "Platform Team"
      }
    ]
  }
}
```

## Step 3: Start MongoDB

```bash
# From ErrorPatternAnalyzer directory
docker-compose up -d

# Verify MongoDB is running
docker ps
```

You can access MongoDB Express at http://localhost:8081 (admin/admin)

## Step 4: Build & Run

```bash
# Build solution
dotnet build ErrorPatternAnalyzer.sln

# Run worker
cd src\ErrorPatternAnalyzer.Worker
dotnet run
```

## Step 5: Verify

You should see output like:

```
[10:30:00 INF] Error Pattern Analyzer Worker starting...
[10:30:01 INF] Querying New Relic for errors from 'Registration-API' since 2025-10-22T10:15:00
[10:30:02 INF] ✓ New Relic connection successful
[10:30:02 INF] Fetched 1234 errors from 'Registration-API'
[10:30:02 INF] Converted 1234 New Relic errors to ErrorEntry models
```

## Troubleshooting

### "New Relic connection failed"
- Verify Account ID is correct
- Verify API Key starts with `NRAK-`
- Check that Query Key has proper permissions

### "No errors found"
- Check Application Name matches exactly in New Relic
- Verify there are errors in the lookback window (default 15 minutes)

### "MongoDB connection failed"
- Ensure Docker is running: `docker ps`
- Check port 27017 is not in use
- Restart MongoDB: `docker-compose restart mongodb`

## Next Steps

1. **Configure Multiple Applications**: Add more apps to the `Applications` array
2. **Setup Slack Alerts**: Add Slack webhook URL to configuration
3. **Run API Dashboard**: (Coming soon)
4. **Configure ML Clustering**: (Coming soon)

## Logs

- Console output: Real-time logs
- File logs: `src/ErrorPatternAnalyzer.Worker/logs/worker-{date}.txt`

## Stopping

```bash
# Stop worker: Ctrl+C

# Stop MongoDB
docker-compose down
```
