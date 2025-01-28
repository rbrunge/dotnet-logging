using Azure.Monitor.OpenTelemetry.AspNetCore;
using CommonLogging;
using OpenTelemetry.Instrumentation.AspNetCore;
using Scalar.AspNetCore;

// based on: https://learn.microsoft.com/en-us/azure/azure-monitor/app/opentelemetry-dotnet-migrate?tabs=aspnetcore

var builder = WebApplication.CreateBuilder(args);

// Some part of .UseAzureMonitor() is reading the connection string from environment variables and not 
// from appsettings.json. This is a workaround to set the connection string in environment variables if it is not set.
// https://github.com/Azure/azure-sdk-for-net/blob/main/sdk/monitor/Azure.Monitor.OpenTelemetry.AspNetCore/src/OpenTelemetryBuilderExtensions.cs
if (Environment.GetEnvironmentVariable("APPLICATIONINSIGHTS_CONNECTION_STRING") == null)
    Environment.SetEnvironmentVariable("APPLICATIONINSIGHTS_CONNECTION_STRING",
        builder.Configuration["ApplicationInsights:ConnectionString"]);

builder.Services.AddOpenApi();
builder.Services.AddOpenTelemetry()
    .UseAzureMonitor();

builder.Services.Configure<AspNetCoreTraceInstrumentationOptions>(options =>
{
    options.RecordException = true;
    // only collect telemetry about HTTP GET requests
    options.Filter = httpContext => HttpMethods.IsGet(httpContext.Request.Method);
});

var app = builder.Build();

app.MapOpenApi();
app.MapScalarApiReference(options => options.WithTitle("04 from Program.cs")); //  => https://localhost:7030/scalar/v1

app.UseHttpsRedirection();

app.AddLoggingApi();

app.Run();