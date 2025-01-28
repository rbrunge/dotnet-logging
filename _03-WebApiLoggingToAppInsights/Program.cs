using CommonLogging;
using Microsoft.Extensions.Logging.ApplicationInsights;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Some part of .AddApplicationInsightsTelemetry() is reading the connection string from environment variables and not 
// from appsettings.json. This is a workaround to set the connection string in environment variables if it is not set.
if (Environment.GetEnvironmentVariable("APPLICATIONINSIGHTS_CONNECTION_STRING") == null)
    Environment.SetEnvironmentVariable("APPLICATIONINSIGHTS_CONNECTION_STRING",
        builder.Configuration["ApplicationInsights:ConnectionString"]);

builder.Services.AddApplicationInsightsTelemetry();

// Adding this filter makes LogInformation turn up for namespace CommonLogging. This can only be set in code, not in appsettings.json.
// But only logs from CommonLogging namespace are shown. Note: looks like "" means the rest of the namespaces.
builder.Logging.AddFilter<ApplicationInsightsLoggerProvider>("CommonLogging", LogLevel.Information);
builder.Logging.AddFilter<ApplicationInsightsLoggerProvider>("", LogLevel.Warning);

builder.Services.AddOpenApi();

var app = builder.Build();

app.MapOpenApi();
app.MapScalarApiReference(options => //  => https://localhost:7086/scalar/v1
{
    options.WithTitle("03 from Program.cs");
});

app.UseHttpsRedirection();

app.AddLoggingApi();

app.Run();