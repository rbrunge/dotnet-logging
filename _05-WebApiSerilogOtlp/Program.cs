using Azure.Monitor.OpenTelemetry.AspNetCore;
using Azure.Monitor.OpenTelemetry.Exporter;
using CommonLogging;
using Scalar.AspNetCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

if (Environment.GetEnvironmentVariable("APPLICATIONINSIGHTS_CONNECTION_STRING") == null)
    Environment.SetEnvironmentVariable("APPLICATIONINSIGHTS_CONNECTION_STRING",
        builder.Configuration["ApplicationInsights:ConnectionString"]);

builder.Services.AddOpenTelemetry()
    .UseAzureMonitor();


builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration));
builder.Services.AddOpenApi();

// NOTE: this is not working!
// App wont run when both .UseSerilog() and .UseAzureMonitor() are used
// Cannot get passed "Loading ..." shown in threads window


//
                          // builder.Services.AddOpenTelemetry()
                          //     .WithTracing(x => x
                          //         .AddAzureMonitorTraceExporter(o =>
                          //         {
                          //             o.ConnectionString = Environment.GetEnvironmentVariable("APPLICATIONINSIGHTS_CONNECTION_STRING");
                          //         }))
                          //     // .WithMetrics(x => x.AddMeter(InstrumentationOptions.MeterName)
                          //     //
                          //     //     .AddAzureMonitorMetricExporter(o =>
                          //     //
                          //     //     {
                          //     //
                          //     //         o.ConnectionString = Environment.GetEnvironmentVariable("APPLICATIONINSIGHTS_CONNECTION_STRING");
                          //     //
                          //     //     }))
                          //     .UseAzureMonitor();

var app = builder.Build();

app.MapOpenApi();
app.MapScalarApiReference(options => options.WithTitle("05 from Program.cs")); //  => https://localhost:7236/scalar/v1

app.UseHttpsRedirection();
app.AddLoggingApi();

app.Run();