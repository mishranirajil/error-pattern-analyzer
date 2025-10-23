using ErrorPatternAnalyzer.Infrastructure.NewRelic;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace ErrorPatternAnalyzer.Tests;

public class NewRelicClientTests
{
	[Fact]
	public void NewRelicConfig_Validate_ThrowsWhenAccountIdMissing()
	{
		// Arrange
		var config = new NewRelicConfig
		{
			AccountId = "",
			ApiKey = "NRAK-test"
		};

		// Act & Assert
		var exception = Assert.Throws<InvalidOperationException>(() => config.Validate());
		Assert.Contains("AccountId", exception.Message);
	}

	[Fact]
	public void NewRelicConfig_Validate_ThrowsWhenApiKeyMissing()
	{
		// Arrange
		var config = new NewRelicConfig
		{
			AccountId = "12345",
			ApiKey = ""
		};

		// Act & Assert
		var exception = Assert.Throws<InvalidOperationException>(() => config.Validate());
		Assert.Contains("ApiKey", exception.Message);
	}

	[Fact]
	public void NewRelicConfig_Validate_ThrowsWhenApiKeyInvalid()
	{
		// Arrange
		var config = new NewRelicConfig
		{
			AccountId = "12345",
			ApiKey = "invalid-key"
		};

		// Act & Assert
		var exception = Assert.Throws<InvalidOperationException>(() => config.Validate());
		Assert.Contains("NRAK-", exception.Message);
	}

	[Fact]
	public void NewRelicConfig_GetEnabledApplications_ReturnsSingleAppMode()
	{
		// Arrange
		var config = new NewRelicConfig
		{
			AccountId = "12345",
			ApiKey = "NRAK-test",
			ApplicationName = "TestApp"
		};

		// Act
		var apps = config.GetEnabledApplications();

		// Assert
		Assert.Single(apps);
		Assert.Equal("TestApp", apps[0].Name);
	}

	[Fact]
	public void NewRelicConfig_GetEnabledApplications_ReturnsOnlyEnabled()
	{
		// Arrange
		var config = new NewRelicConfig
		{
			AccountId = "12345",
			ApiKey = "NRAK-test",
			Applications = new List<ApplicationConfig>
			{
				new() { Name = "App1", Enabled = true },
				new() { Name = "App2", Enabled = false },
				new() { Name = "App3", Enabled = true }
			}
		};

		// Act
		var apps = config.GetEnabledApplications();

		// Assert
		Assert.Equal(2, apps.Count);
		Assert.Contains(apps, a => a.Name == "App1");
		Assert.Contains(apps, a => a.Name == "App3");
	}
}
