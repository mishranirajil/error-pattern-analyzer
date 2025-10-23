using System.Net.Http.Headers;
using System.Net.Http.Json;
using ErrorPatternAnalyzer.Infrastructure.NewRelic.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ErrorPatternAnalyzer.Infrastructure.NewRelic;

/// <summary>
/// Client for interacting with New Relic Insights Query API
/// </summary>
public class NewRelicClient
{
	private readonly HttpClient _httpClient;
	private readonly NewRelicConfig _config;
	private readonly ILogger<NewRelicClient> _logger;

	public NewRelicClient(
		HttpClient httpClient,
		IOptions<NewRelicConfig> config,
		ILogger<NewRelicClient> logger)
	{
		_httpClient = httpClient;
		_config = config.Value;
		_logger = logger;

		_config.Validate();

		// Configure HTTP client
		_httpClient.BaseAddress = new Uri(_config.QueryApiUrl);
		_httpClient.DefaultRequestHeaders.Add("X-Query-Key", _config.ApiKey);
		_httpClient.DefaultRequestHeaders.Accept.Add(
			new MediaTypeWithQualityHeaderValue("application/json"));
		_httpClient.Timeout = TimeSpan.FromSeconds(_config.QueryTimeout);
	}

	/// <summary>
	/// Fetch error transactions from New Relic for a specific application
	/// </summary>
	public async Task<List<NewRelicError>> FetchErrorsAsync(
		string applicationName,
		DateTime since,
		CancellationToken ct = default)
	{
		try
		{
			var sinceTimestamp = new DateTimeOffset(since).ToUnixTimeMilliseconds();

			// NRQL query to get errors
			var nrql = $@"
				SELECT 
					timestamp,
					error.class,
					error.message,
					`error.expected`,
					httpResponseCode,
					host,
					name,
					duration,
					userAgentName,
					request.uri
				FROM TransactionError
				WHERE appName = '{applicationName}'
				  AND timestamp >= {sinceTimestamp}
				LIMIT {_config.MaxErrorsPerPoll}";

			var queryUrl = $"/{_config.AccountId}/query?nrql={Uri.EscapeDataString(nrql)}";

			_logger.LogInformation(
				"Querying New Relic for errors from '{AppName}' since {Since}",
				applicationName,
				since);

			var response = await _httpClient.GetAsync(queryUrl, ct);

			if (!response.IsSuccessStatusCode)
			{
				var errorContent = await response.Content.ReadAsStringAsync(ct);
				_logger.LogError(
					"New Relic API error for '{AppName}': {StatusCode} - {Content}",
					applicationName,
					response.StatusCode,
					errorContent);
				throw new HttpRequestException(
					$"New Relic API returned {response.StatusCode}");
			}

			var result = await response.Content
				.ReadFromJsonAsync<NewRelicQueryResponse>(ct);

			if (result?.Results == null || result.Results.Count == 0)
			{
				_logger.LogInformation(
					"No errors found for '{AppName}'",
					applicationName);
				return new List<NewRelicError>();
			}

			_logger.LogInformation(
				"Fetched {Count} errors from '{AppName}'",
				result.Results.Count,
				applicationName);

			return result.Results;
		}
		catch (Exception ex)
		{
			_logger.LogError(
				ex,
				"Failed to fetch errors from New Relic for '{AppName}'",
				applicationName);
			throw;
		}
	}

	/// <summary>
	/// Test connection to New Relic API
	/// </summary>
	public async Task<bool> TestConnectionAsync(CancellationToken ct = default)
	{
		try
		{
			var nrql = "SELECT count(*) FROM Transaction SINCE 1 minute ago LIMIT 1";
			var queryUrl = $"/{_config.AccountId}/query?nrql={Uri.EscapeDataString(nrql)}";

			var response = await _httpClient.GetAsync(queryUrl, ct);

			if (response.IsSuccessStatusCode)
			{
				_logger.LogInformation("New Relic connection test successful");
				return true;
			}

			_logger.LogWarning(
				"New Relic connection test failed: {StatusCode}",
				response.StatusCode);
			return false;
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "New Relic connection test failed");
			return false;
		}
	}
}
