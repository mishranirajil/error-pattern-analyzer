using System.Text.Json.Serialization;

namespace ErrorPatternAnalyzer.Infrastructure.NewRelic.Models;

public class NewRelicError
{
	[JsonPropertyName("timestamp")]
	public long Timestamp { get; set; }

	[JsonPropertyName("error.class")]
	public string? ErrorClass { get; set; }

	[JsonPropertyName("error.message")]
	public string? ErrorMessage { get; set; }

	[JsonPropertyName("error.expected")]
	public bool? ErrorExpected { get; set; }

	[JsonPropertyName("httpResponseCode")]
	public int? HttpResponseCode { get; set; }

	[JsonPropertyName("host")]
	public string? Host { get; set; }

	[JsonPropertyName("name")]
	public string? TransactionName { get; set; }

	[JsonPropertyName("duration")]
	public double? Duration { get; set; }

	[JsonPropertyName("userAgentName")]
	public string? UserAgent { get; set; }

	[JsonPropertyName("request.uri")]
	public string? RequestUri { get; set; }

	public DateTime GetTimestamp() =>
		DateTimeOffset.FromUnixTimeMilliseconds(Timestamp).UtcDateTime;
}

public class NewRelicQueryResponse
{
	[JsonPropertyName("results")]
	public List<NewRelicError> Results { get; set; } = new();

	[JsonPropertyName("metadata")]
	public NewRelicMetadata? Metadata { get; set; }
}

public class NewRelicMetadata
{
	[JsonPropertyName("eventTypes")]
	public List<string>? EventTypes { get; set; }

	[JsonPropertyName("eventType")]
	public string? EventType { get; set; }

	[JsonPropertyName("openEnded")]
	public bool OpenEnded { get; set; }

	[JsonPropertyName("beginTime")]
	public long BeginTime { get; set; }

	[JsonPropertyName("endTime")]
	public long EndTime { get; set; }
}
