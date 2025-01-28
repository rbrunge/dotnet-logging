using CommonLogging;
using Scalar.AspNetCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Some part of .AddApplicationInsightsTelemetry() is reading the connection string from environment variables and not 
// from appsettings.json. This is a workaround to set the connection string in environment variables if it is not set.
if (Environment.GetEnvironmentVariable("APPLICATIONINSIGHTS_CONNECTION_STRING") == null)
    Environment.SetEnvironmentVariable("APPLICATIONINSIGHTS_CONNECTION_STRING",
        builder.Configuration["ApplicationInsights:ConnectionString"]);

builder.Services.AddApplicationInsightsTelemetry();

builder.Services.AddOpenApi();

// builder.Host.UseSerilog((context, configuration) =>
//     configuration.ReadFrom.Configuration(context.Configuration));


var app = builder.Build();
//app.UseSerilogRequestLogging();

app.MapOpenApi();
app.MapScalarApiReference(options => options.WithTitle("06 from Program.cs")); // => https://localhost:7048/scalar/v1

app.UseHttpsRedirection();

app.AddLoggingApi();

app.Run();