using CommonLogging;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Extensions.Logging.ApplicationInsights;
using Scalar.AspNetCore;
using Serilog;
using Log = Serilog.Log;

// This enables logging during the startup of the application.
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

var builder = WebApplication.CreateBuilder(args);

// Some part of .AddApplicationInsightsTelemetry() is reading the connection string from environment variables and not 
// from appsettings.json. This is a workaround to set the connection string in environment variables if it is not set.
if (Environment.GetEnvironmentVariable("APPLICATIONINSIGHTS_CONNECTION_STRING") == null)
    Environment.SetEnvironmentVariable("APPLICATIONINSIGHTS_CONNECTION_STRING",
        builder.Configuration["ApplicationInsights:ConnectionString"]);

// This is the classic way to add Application Insights.
// Serilog uses TelemetryConfiguration later that is set up by AddApplicationInsightsTelemetry.
builder.Services.AddApplicationInsightsTelemetry();

// This makes LogInformation turn up for namespace CommonLogging. This can only be set in code, not in appsettings.json.
builder.Logging.AddFilter<ApplicationInsightsLoggerProvider>("CommonLogging", LogLevel.Information);
builder.Logging.AddFilter<ApplicationInsightsLoggerProvider>("", LogLevel.Warning);

builder.Host.UseSerilog();

builder.Services.AddOpenApi();

var app = builder.Build();

var telemetryConfiguration = app.Services.GetRequiredService<TelemetryConfiguration>();
// Here application insight is explicitly added to Serilog, this is needed
// since the telemetryConfiguration is not automatically picked up by Serilog or read
// from appSettings.json.
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .WriteTo.ApplicationInsights(
        telemetryConfiguration,
        TelemetryConverter.Traces)
    .CreateLogger(); 

// This adds metrics to http requests. Adds something like this:
//   `HTTP GET /api/test/log-all responded 200 in 37.4184 ms`
app.UseSerilogRequestLogging();

app.MapOpenApi();
app.MapScalarApiReference(options => options.WithTitle("06 from Program.cs")); // => https://localhost:7048/scalar/v1

app.UseHttpsRedirection();

app.AddLoggingApi();

app.Run();