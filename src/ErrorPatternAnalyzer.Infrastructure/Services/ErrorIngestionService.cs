using ErrorPatternAnalyzer.Core.Models;
using ErrorPatternAnalyzer.Core.Services;
using ErrorPatternAnalyzer.Infrastructure.NewRelic;
using Microsoft.Extensions.Logging;

namespace ErrorPatternAnalyzer.Infrastructure.Services;

public class ErrorIngestionService : IErrorIngestionService
{
	private readonly NewRelicClient _newRelicClient;
	private readonly ILogger<ErrorIngestionService> _logger;

	public ErrorIngestionService(
		NewRelicClient newRelicClient,
		ILogger<ErrorIngestionService> logger)
	{
		_newRelicClient = newRelicClient;
		_logger = logger;
	}

	public async Task<List<ErrorEntry>> IngestFromNewRelicAsync(
		string applicationName,
		DateTime since,
		CancellationToken ct = default)
	{
		try
		{
			_logger.LogInformation(
				"Ingesting errors from New Relic for '{AppName}' since {Since}",
				applicationName,
				since);

			var newRelicErrors = await _newRelicClient.FetchErrorsAsync(
				applicationName,
				since,
				ct);

			if (newRelicErrors.Count == 0)
			{
				_logger.LogInformation("No errors to ingest for '{AppName}'", applicationName);
				return new List<ErrorEntry>();
			}

			// Convert New Relic errors to ErrorEntry domain models
			var errorEntries = newRelicErrors.Select(nr => new ErrorEntry
			{
				Id = Guid.NewGuid().ToString(),
				Timestamp = nr.GetTimestamp(),
				Message = nr.ErrorMessage ?? "No message",
				ExceptionType = nr.ErrorClass ?? "Unknown",
				Source = "NewRelic",
				ApplicationName = applicationName,
				StatusCode = nr.HttpResponseCode ?? 0,
				Endpoint = nr.TransactionName ?? nr.RequestUri,
				Duration = nr.Duration ?? 0,
				UserAgent = nr.UserAgent,
				Host = nr.Host,
				Context = new Dictionary<string, object>
				{
					["host"] = nr.Host ?? "unknown",
					["isExpected"] = nr.ErrorExpected ?? false,
					["requestUri"] = nr.RequestUri ?? ""
				}
			}).ToList();

			_logger.LogInformation(
				"Converted {Count} New Relic errors to ErrorEntry models for '{AppName}'",
				errorEntries.Count,
				applicationName);

			return errorEntries;
		}
		catch (Exception ex)
		{
			_logger.LogError(
				ex,
				"Failed to ingest errors from New Relic for '{AppName}'",
				applicationName);
			throw;
		}
	}

	public async Task<List<ErrorEntry>> IngestAllApplicationsAsync(
		DateTime since,
		CancellationToken ct = default)
	{
		// This will be implemented when we have access to config
		// For now, throw NotImplementedException
		await Task.CompletedTask;
		throw new NotImplementedException("Multi-application ingestion not yet implemented");
	}
}
