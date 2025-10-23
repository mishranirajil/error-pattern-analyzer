# Contributing to Error Pattern Analyzer

## Development Setup

### Prerequisites
- .NET 8 SDK
- Docker Desktop
- Visual Studio 2022 or VS Code
- Git

### Getting Started

1. **Clone the repository:**
   ```bash
   git clone <repository-url>
   cd error-pattern-analyzer
   ```

2. **Install dependencies:**
   ```bash
   dotnet restore ErrorPatternAnalyzer.sln
   ```

3. **Start local infrastructure:**
   ```bash
   docker-compose up -d
   ```

4. **Build:**
   ```bash
   dotnet build ErrorPatternAnalyzer.sln
   ```

5. **Run tests:**
   ```bash
   dotnet test ErrorPatternAnalyzer.sln
   ```

## Coding Standards

### .editorconfig
This project uses an `.editorconfig` file to maintain consistent coding standards:
- **Indentation:** Tabs (size 4)
- **Line endings:** CRLF
- **Nullable reference types:** Enabled
- **Warnings as errors:** Enabled in production builds

### Naming Conventions
- **Interfaces:** Start with `I` (e.g., `IErrorRepository`)
- **Classes:** PascalCase (e.g., `ErrorClusteringEngine`)
- **Methods:** PascalCase, async methods end with `Async`
- **Private fields:** `_camelCase` with underscore prefix
- **Constants:** PascalCase

### Code Organization
```
src/
  ProjectName/
    Controllers/      # API controllers
    Services/         # Business logic
    Models/           # Domain models
    Infrastructure/   # External integrations

tests/
  ProjectName.Tests/
    Unit/            # Unit tests
    Integration/     # Integration tests
```

## Branching Strategy

### Branch Names
- `main` - Production-ready code
- `develop` - Integration branch for features
- `feature/description` - New features
- `bugfix/description` - Bug fixes
- `hotfix/description` - Urgent production fixes

### Workflow
1. Create feature branch from `develop`
2. Make changes and commit
3. Push and create pull request
4. Wait for CI/CD checks to pass
5. Request code review
6. Merge after approval

## Pull Request Guidelines

### PR Title Format
```
[Type] Brief description

Types: Feature, Bugfix, Hotfix, Refactor, Docs, Test
Example: [Feature] Add ML clustering engine
```

### PR Description Template
```markdown
## What
Brief description of changes

## Why
Reason for the change

## How
Technical approach

## Testing
- [ ] Unit tests added/updated
- [ ] Integration tests added/updated
- [ ] Manual testing completed

## Checklist
- [ ] Code follows .editorconfig standards
- [ ] All tests pass
- [ ] No new warnings
- [ ] Documentation updated
```

## Testing Guidelines

### Unit Tests
- Use xUnit framework
- Mock external dependencies
- Test one thing per test
- Use descriptive test names

**Example:**
```csharp
[Fact]
public void NewRelicConfig_Validate_ThrowsWhenAccountIdMissing()
{
    // Arrange
    var config = new NewRelicConfig { AccountId = "" };
    
    // Act & Assert
    Assert.Throws<InvalidOperationException>(() => config.Validate());
}
```

### Integration Tests
- Use Docker containers for dependencies
- Clean up test data after each test
- Use realistic test data
- Test complete workflows

## Documentation

### Code Comments
- Use XML documentation for public APIs
- Explain *why*, not *what*
- Keep comments up-to-date

**Example:**
```csharp
/// <summary>
/// Clusters errors using TF-IDF vectorization and cosine similarity.
/// This approach works well for text-heavy error messages.
/// </summary>
/// <param name="errors">Errors to cluster</param>
/// <returns>Clustered errors with similarity scores</returns>
public async Task<IEnumerable<ErrorCluster>> ClusterErrorsAsync(...)
```

### README Updates
- Update README when adding features
- Keep quickstart guide accurate
- Add troubleshooting entries for common issues

## Performance Guidelines

### Database Queries
- Use indexed fields in MongoDB queries
- Limit result sets appropriately
- Avoid N+1 query problems

### Async/Await
- Use async methods for I/O operations
- Don't block on async calls with `.Result` or `.Wait()`
- Use `ConfigureAwait(false)` in library code

### Logging
- Use structured logging with Serilog
- Log at appropriate levels (Debug, Info, Warning, Error)
- Don't log sensitive information

## Security Guidelines

### Credentials
- Never commit credentials to Git
- Use User Secrets for local development
- Use environment variables in production
- Rotate keys regularly

### Input Validation
- Validate all external input
- Use parameterized queries
- Sanitize user input for display

### Dependencies
- Keep NuGet packages up-to-date
- Review security advisories
- Use Dependabot for automated updates

## Release Process

### Version Numbers
Follow Semantic Versioning (SemVer):
- **Major:** Breaking changes
- **Minor:** New features (backward compatible)
- **Patch:** Bug fixes

### Release Checklist
- [ ] All tests pass
- [ ] Documentation updated
- [ ] CHANGELOG.md updated
- [ ] Version number bumped
- [ ] Tag created in Git
- [ ] Release notes written

## Getting Help

### Resources
- **Documentation:** See `/docs` folder
- **Troubleshooting:** See `TROUBLESHOOTING.md`
- **Architecture:** See `IMPLEMENTATION-STATUS.md`

### Contact
- Team Lead: [Contact Info]
- Architecture Questions: [Contact Info]
- Security Issues: security@imaginelearning.com

## Code Review Guidelines

### As a Reviewer
- Review within 24 hours
- Provide constructive feedback
- Test locally if possible
- Check for security issues
- Verify tests are comprehensive

### As an Author
- Keep PRs small and focused
- Respond to feedback promptly
- Don't take feedback personally
- Update based on comments
- Re-request review after changes

---

**Thank you for contributing to Error Pattern Analyzer!**
