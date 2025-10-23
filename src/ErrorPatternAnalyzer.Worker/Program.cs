using ErrorPatternAnalyzer.Core.Services;
using ErrorPatternAnalyzer.Infrastructure.NewRelic;
using ErrorPatternAnalyzer.Infrastructure.Services;
using Serilog;

var builder = Host.CreateApplicationBuilder(args);

// Configure Serilog
Log.Logger = new LoggerConfiguration()
	.ReadFrom.Configuration(builder.Configuration)
	.WriteTo.Console()
	.WriteTo.File("logs/worker-.txt", rollingInterval: RollingInterval.Day)
	.CreateLogger();

builder.Services.AddSerilog();

// Configure New Relic
builder.Services.Configure<NewRelicConfig>(
	builder.Configuration.GetSection("NewRelic"));
builder.Services.AddHttpClient<NewRelicClient>();

// Register services
builder.Services.AddSingleton<IErrorIngestionService, ErrorIngestionService>();

// TODO: Add MongoDB, clustering, pattern detection, and alerting services

// Register the worker
// builder.Services.AddHostedService<ErrorAnalysisWorker>();

var host = builder.Build();

try
{
	Log.Information("Error Pattern Analyzer Worker starting...");

	// Test New Relic connection
	var newRelicClient = host.Services.GetRequiredService<NewRelicClient>();
	var connectionTest = await newRelicClient.TestConnectionAsync();

	if (connectionTest)
	{
		Log.Information("✓ New Relic connection successful");
	}
	else
	{
		Log.Warning("✗ New Relic connection failed - check configuration");
	}

	await host.RunAsync();
}
catch (Exception ex)
{
	Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
	Log.CloseAndFlush();
}
