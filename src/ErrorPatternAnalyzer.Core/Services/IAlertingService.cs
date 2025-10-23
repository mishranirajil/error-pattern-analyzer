using ErrorPatternAnalyzer.Core.Models;

namespace ErrorPatternAnalyzer.Core.Services;

/// <summary>
/// Service for sending alerts about error patterns
/// </summary>
public interface IAlertingService
{
	/// <summary>
	/// Send alert for a specific pattern
	/// </summary>
	Task SendPatternAlertAsync(
		ErrorPattern pattern,
		string? channel = null,
		CancellationToken ct = default);

	/// <summary>
	/// Send daily digest of patterns
	/// </summary>
	Task SendDailyDigestAsync(
		DateTime since,
		CancellationToken ct = default);

	/// <summary>
	/// Test alerting configuration
	/// </summary>
	Task<bool> TestAlertAsync(
		string channel,
		CancellationToken ct = default);
}
