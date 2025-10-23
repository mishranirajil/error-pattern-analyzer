using System.ComponentModel.DataAnnotations;

namespace ErrorPatternAnalyzer.Core.Models;

/// <summary>
/// Represents a cluster of similar errors
/// </summary>
public class ErrorCluster
{
	[Required]
	public string Id { get; set; } = Guid.NewGuid().ToString();

	[Required]
	public string PatternSignature { get; set; } = string.Empty;

	[Required]
	public string RepresentativeError { get; set; } = string.Empty;

	[Required]
	public List<string> ErrorIds { get; set; } = new();

	public int Occurrences => ErrorIds.Count;

	[Required]
	public DateTime FirstSeen { get; set; }

	[Required]
	public DateTime LastSeen { get; set; }

	[Required]
	public ErrorSeverity Severity { get; set; } = ErrorSeverity.Low;

	public string? SuggestedRootCause { get; set; }

	public List<string> AffectedUsers { get; set; } = new();

	public List<string> AffectedEndpoints { get; set; } = new();

	public string ApplicationName { get; set; } = string.Empty;

	public string? Repository { get; set; }

	public string? Team { get; set; }

	public DateTime Created { get; set; } = DateTime.UtcNow;

	public DateTime Updated { get; set; } = DateTime.UtcNow;
}

public enum ErrorSeverity
{
	Low,
	Medium,
	High,
	Critical
}
