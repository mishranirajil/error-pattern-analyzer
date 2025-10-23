using ErrorPatternAnalyzer.Core.Models;

namespace ErrorPatternAnalyzer.Core.Services;

/// <summary>
/// Service for detecting patterns in error clusters
/// </summary>
public interface IPatternDetector
{
	/// <summary>
	/// Detect patterns from error clusters
	/// </summary>
	Task<IEnumerable<ErrorPattern>> DetectPatternsAsync(
		IEnumerable<ErrorCluster> clusters,
		CancellationToken ct = default);

	/// <summary>
	/// Analyze trend for a specific pattern
	/// </summary>
	Task<PatternTrend> AnalyzeTrendAsync(
		string patternId,
		TimeSpan timeWindow,
		CancellationToken ct = default);
}

public class PatternTrend
{
	public string PatternId { get; set; } = string.Empty;
	public TrendDirection Direction { get; set; }
	public double ChangeRate { get; set; }
	public bool IsAccelerating { get; set; }
	public int ForecastNextPeriod { get; set; }
}

public enum TrendDirection
{
	Increasing,
	Decreasing,
	Stable
}
