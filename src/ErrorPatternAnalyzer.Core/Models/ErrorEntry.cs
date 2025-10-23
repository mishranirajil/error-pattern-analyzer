using System.ComponentModel.DataAnnotations;

namespace ErrorPatternAnalyzer.Core.Models;

/// <summary>
/// Represents a single error entry ingested from New Relic
/// </summary>
public class ErrorEntry
{
	[Required]
	public string Id { get; set; } = Guid.NewGuid().ToString();

	[Required]
	public DateTime Timestamp { get; set; }

	[Required]
	public string Message { get; set; } = string.Empty;

	public string? StackTrace { get; set; }

	[Required]
	public string ExceptionType { get; set; } = string.Empty;

	[Required]
	public string Source { get; set; } = "NewRelic";

	// Application Context
	[Required]
	public string ApplicationName { get; set; } = string.Empty;

	public string? Repository { get; set; }

	public string? Team { get; set; }

	// HTTP Context
	public int StatusCode { get; set; }

	public string? Endpoint { get; set; }

	public double Duration { get; set; }

	public string? UserAgent { get; set; }

	public string? Host { get; set; }

	// Additional Context
	public Dictionary<string, object> Context { get; set; } = new();

	// Clustering
	public string? ClusterId { get; set; }

	public DateTime Created { get; set; } = DateTime.UtcNow;
}
