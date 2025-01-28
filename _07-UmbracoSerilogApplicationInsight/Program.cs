using CommonLogging;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Extensions.Logging.ApplicationInsights;
using Scalar.AspNetCore;
using Serilog;
using Log = Serilog.Log;

// Status:
// - Cannot see my own minimal APIs in Swagger UI BUT scalar is up https://localhost:44373/scalar
// - Minimal APIs do work. https://localhost:44373/api/test/log-all
// - Having, app.UseSerilogRequestLogging(); in the code, adds too much noise. Need to control y namespace
// - TODO: try this -> https://docs.umbraco.com/umbraco-cms/reference/api-versioning-and-openapi

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.CreateUmbracoBuilder()
    .AddBackOffice()
    .AddWebsite()
    .AddComposers()
    .Build();

// Some part of .AddApplicationInsightsTelemetry() is reading the connection string from environment variables and not 
// from appsettings.json. This is a workaround to set the connection string in environment variables if it is not set.
if (Environment.GetEnvironmentVariable("APPLICATIONINSIGHTS_CONNECTION_STRING") == null)
    Environment.SetEnvironmentVariable("APPLICATIONINSIGHTS_CONNECTION_STRING",
        builder.Configuration["ApplicationInsights:ConnectionString"]);

// This is the classic way to add Application Insights.
// Serilog uses TelemetryConfiguration later that is set up by AddApplicationInsightsTelemetry.
builder.Services.AddApplicationInsightsTelemetry();

// TODO roru - check, not working - other namespaces are logged
builder.Logging.AddFilter<ApplicationInsightsLoggerProvider>("CommonLogging", LogLevel.Information);
builder.Logging.AddFilter<ApplicationInsightsLoggerProvider>("", LogLevel.Warning);

builder.Host.UseSerilog();

builder.Services.AddOpenApi();

WebApplication app = builder.Build();

await app.BootUmbracoAsync();

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
//app.UseSerilogRequestLogging();

app.MapOpenApi(); // /openapi/v1.json
app.MapScalarApiReference(o => o.WithTitle("07 Umbraco")); //  => /scalar/v1

app.AddLoggingApi();

app.UseUmbraco()
    .WithMiddleware(u =>
    {
        u.UseBackOffice();
        u.UseWebsite();
    })
    .WithEndpoints(u =>
    {
        u.UseBackOfficeEndpoints();
        u.UseWebsiteEndpoints();
    });


await app.RunAsync();
