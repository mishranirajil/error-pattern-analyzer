using ErrorPatternAnalyzer.Core.Models;

namespace ErrorPatternAnalyzer.Core.Services;

/// <summary>
/// Service for ingesting errors from external sources
/// </summary>
public interface IErrorIngestionService
{
	/// <summary>
	/// Ingest errors from New Relic for a specific application
	/// </summary>
	Task<List<ErrorEntry>> IngestFromNewRelicAsync(
		string applicationName,
		DateTime since,
		CancellationToken ct = default);

	/// <summary>
	/// Ingest errors from New Relic for all configured applications
	/// </summary>
	Task<List<ErrorEntry>> IngestAllApplicationsAsync(
		DateTime since,
		CancellationToken ct = default);
}
