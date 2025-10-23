using System.ComponentModel.DataAnnotations;

namespace ErrorPatternAnalyzer.Core.Models;

/// <summary>
/// Represents a detected error pattern
/// </summary>
public class ErrorPattern
{
	[Required]
	public string Id { get; set; } = Guid.NewGuid().ToString();

	[Required]
	public string Name { get; set; } = string.Empty;

	public string? Description { get; set; }

	[Required]
	public List<string> ClusterIds { get; set; } = new();

	[Required]
	public PatternType Type { get; set; } = PatternType.Persistent;

	public double Confidence { get; set; }

	public string? PotentialRootCause { get; set; }

	public List<string> RelatedPatterns { get; set; } = new();

	[Required]
	public DateTime IdentifiedAt { get; set; }

	public PatternStatus Status { get; set; } = PatternStatus.Active;

	public string? AssignedTo { get; set; }

	public string? ResolutionNotes { get; set; }

	public DateTime? ResolvedAt { get; set; }

	public string ApplicationName { get; set; } = string.Empty;

	public string? Repository { get; set; }

	public string? Team { get; set; }

	public int Occurrences { get; set; }

	public bool IsNew { get; set; } = true;

	public DateTime Created { get; set; } = DateTime.UtcNow;

	public DateTime Updated { get; set; } = DateTime.UtcNow;
}

public enum PatternType
{
	Transient,      // Short-lived, self-resolving
	Persistent,     // Ongoing, needs attention
	Trending,       // Increasing frequency
	Cyclic,         // Repeats on schedule
	Correlated      // Appears with other patterns
}

public enum PatternStatus
{
	Active,
	UnderInvestigation,
	Resolved,
	Ignored
}
