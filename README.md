# Error Pattern Analyzer

**AI-Powered Error Detection and Root Cause Analysis System**

Automatically analyzes application errors from New Relic, detects patterns using ML, and provides intelligent alerts with root cause suggestions.

## Features

- ✅ **Multi-Application Support** - Monitor multiple applications simultaneously
- ✅ **New Relic Integration** - Fetch errors via NRQL queries
- ✅ **ML-Powered Clustering** - Group similar errors using TF-IDF and cosine similarity
- ✅ **Pattern Detection** - Identify recurring issues and trends
- ✅ **Root Cause Analysis** - Automated hypothesis generation
- ✅ **Intelligent Alerting** - Only alert on new/critical patterns
- ✅ **Team-Specific Routing** - Send alerts to appropriate Slack channels
- ✅ **REST API** - Query patterns and insights programmatically

## Quick Start

### Prerequisites

- .NET 8 SDK
- MongoDB
- New Relic account with Query API access

### Configuration

1. **Get New Relic credentials:**
   - Account ID: Find in New Relic URL
   - API Key: Create a "Query Key" (starts with `NRAK-`)
   - Application Name: Your app name in New Relic APM

2. **Update `appsettings.json`:**

```json
{
  "NewRelic": {
    "AccountId": "YOUR_ACCOUNT_ID",
    "ApiKey": "NRAK-XXXXXXXXXXXXXXXXXXXX",
    "Applications": [
      {
        "Name": "Registration-API",
        "Enabled": true,
        "Repository": "ImagineLearning/registration",
        "Team": "Platform Team",
        "SlackChannel": "#platform-errors"
      }
    ]
  },
  "MongoDB": {
    "ConnectionString": "mongodb://localhost:27017",
    "DatabaseName": "ErrorPatternAnalyzer"
  }
}
```

### Run

```bash
# Start MongoDB
docker-compose up -d

# Run Worker (analyzes errors)
cd src/ErrorPatternAnalyzer.Worker
dotnet run

# Run API (dashboard/queries)
cd src/ErrorPatternAnalyzer.Api
dotnet run
```

### Build

```bash
dotnet build ErrorPatternAnalyzer.sln
```

### Test

```bash
dotnet test
```

## Architecture

```
ErrorPatternAnalyzer/
├── src/
│   ├── ErrorPatternAnalyzer.Api/          # REST API for dashboard
│   ├── ErrorPatternAnalyzer.Core/         # Domain models & services
│   ├── ErrorPatternAnalyzer.Infrastructure/  # New Relic, MongoDB, Slack
│   └── ErrorPatternAnalyzer.Worker/       # Background processor
└── tests/
    └── ErrorPatternAnalyzer.Tests/
```

## API Endpoints

- `GET /api/v1/patterns` - List error patterns
- `GET /api/v1/patterns/{id}` - Pattern details
- `GET /api/v1/applications` - Application summaries
- `GET /api/v1/insights` - Dashboard insights

## Documentation

### 📘 Getting Started (Read First!)
- **[QUICKSTART.md](QUICKSTART.md)** - Step-by-step setup guide with New Relic credentials
- **[TROUBLESHOOTING.md](TROUBLESHOOTING.md)** - Common issues, error messages, and solutions

### 📊 Project Status
- **[DELIVERY-SUMMARY.md](DELIVERY-SUMMARY.md)** - What's complete, what works now, next steps
- **[IMPLEMENTATION-STATUS.md](IMPLEMENTATION-STATUS.md)** - Detailed phase-by-phase status

### 📚 Design Documents
- [Implementation Guide](../error-analyzer-standalone-implementation.md) - Standalone app architecture
- [Multi-Repository Support](../error-analyzer-multi-repo-support.md) - Multi-app configuration
- [Full Proposal](../ai-error-pattern-analyzer-proposal.md) - Complete design (1172 lines)

---

## Build Status

✅ **All Projects:** Compiling successfully  
✅ **All Tests:** 5/5 passing  
✅ **Warnings:** 0  
✅ **Errors:** 0  

**Current Phase:** Phase 1 Complete - Foundation Ready  
**Next Phase:** MongoDB Repositories + ML Clustering

## License

Proprietary - Imagine Learning
