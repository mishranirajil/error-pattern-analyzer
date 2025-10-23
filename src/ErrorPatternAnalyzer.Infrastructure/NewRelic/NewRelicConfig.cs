namespace ErrorPatternAnalyzer.Infrastructure.NewRelic;

/// <summary>
/// Configuration for New Relic API integration
/// </summary>
public class NewRelicConfig
{
	public string AccountId { get; set; } = string.Empty;

	public string ApiKey { get; set; } = string.Empty;

	public string QueryApiUrl { get; set; } = "https://insights-api.newrelic.com/v1/accounts";

	/// <summary>
	/// Single application mode (for backward compatibility)
	/// </summary>
	public string? ApplicationName { get; set; }

	/// <summary>
	/// Multi-application mode (recommended)
	/// </summary>
	public List<ApplicationConfig> Applications { get; set; } = new();

	public int PollingIntervalMinutes { get; set; } = 5;

	public int MaxErrorsPerPoll { get; set; } = 10000;

	public int LookbackMinutes { get; set; } = 15;

	public int QueryTimeout { get; set; } = 30;

	public void Validate()
	{
		if (string.IsNullOrEmpty(AccountId))
		{
			throw new InvalidOperationException("NewRelic AccountId is required");
		}

		if (string.IsNullOrEmpty(ApiKey))
		{
			throw new InvalidOperationException("NewRelic ApiKey is required");
		}

		if (!ApiKey.StartsWith("NRAK-"))
		{
			throw new InvalidOperationException("NewRelic ApiKey must be a Query Key (starts with NRAK-)");
		}

		// Must have either single app name or multiple applications
		if (string.IsNullOrEmpty(ApplicationName) && Applications.Count == 0)
		{
			throw new InvalidOperationException(
				"Either ApplicationName or Applications list must be specified");
		}

		// Validate enabled applications
		var enabledApps = Applications.Where(a => a.Enabled).ToList();
		if (Applications.Any() && enabledApps.Count == 0)
		{
			throw new InvalidOperationException("At least one application must be enabled");
		}
	}

	/// <summary>
	/// Get all enabled applications
	/// </summary>
	public List<ApplicationConfig> GetEnabledApplications()
	{
		// If using single application mode
		if (!string.IsNullOrEmpty(ApplicationName) && Applications.Count == 0)
		{
			return new List<ApplicationConfig>
			{
				new ApplicationConfig { Name = ApplicationName, Enabled = true }
			};
		}

		// Multi-application mode
		return Applications.Where(a => a.Enabled).ToList();
	}
}

/// <summary>
/// Configuration for a single application
/// </summary>
public class ApplicationConfig
{
	public string Name { get; set; } = string.Empty;

	public bool Enabled { get; set; } = true;

	public string? Repository { get; set; }

	public string? GitHubUrl { get; set; }

	public string? Team { get; set; }

	public string? SlackChannel { get; set; }

	public int MinOccurrencesForAlert { get; set; } = 10;

	public ApplicationFilters? CustomFilters { get; set; }

	public Dictionary<string, string> Metadata { get; set; } = new();
}

/// <summary>
/// Custom filters for an application
/// </summary>
public class ApplicationFilters
{
	public List<int> IgnoreStatusCodes { get; set; } = new();

	public List<string> IgnoreErrorClasses { get; set; } = new();

	public List<string> IgnoreMessagePatterns { get; set; } = new();

	public List<string>? OnlyAlertForEndpoints { get; set; }
}
