using ErrorPatternAnalyzer.Core.Models;

namespace ErrorPatternAnalyzer.Core.Services;

/// <summary>
/// Service for clustering similar errors using ML
/// </summary>
public interface IClusteringEngine
{
	/// <summary>
	/// Cluster a batch of errors by similarity
	/// </summary>
	Task<IEnumerable<ErrorCluster>> ClusterErrorsAsync(
		IEnumerable<ErrorEntry> errors,
		CancellationToken ct = default);

	/// <summary>
	/// Find an existing cluster that matches the given error
	/// </summary>
	Task<ErrorCluster?> FindSimilarClusterAsync(
		ErrorEntry error,
		CancellationToken ct = default);

	/// <summary>
	/// Train/update the clustering model with historical data
	/// </summary>
	Task TrainAsync(
		IEnumerable<ErrorEntry> historicalErrors,
		CancellationToken ct = default);
}
