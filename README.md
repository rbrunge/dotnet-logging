# dotnet-logging

## 00 Console app logging to console
Shows how use LoggerFactory to control LogLevel and log using ILogger. All logging is to console.

## 01 WebApiLocalLogging
Web API that logs to console.
Call GET endpoints to activate the logging:
- /api/test/log-all
- /api/test/throw
- /api/test/throw-try-catch
- /api/test/source-generated

## 02 Web Api logging using serilog

## 03 Web Api logging to app insights (no serilog)

## 04 Web Api logging to app insights using open telemetry
https://learn.microsoft.com/en-us/azure/azure-monitor/app/opentelemetry-enable?tabs=aspnetcore#enable-azure-monitor-opentelemetry-for-net-nodejs-python-and-java-applications

As of 2025-01-27 Microsoft recommends abandoning .AddApplicationInsightsTelemetry() in favor of OpenTelemetry.

## 05 Serilog + Opentelemetry, web api

## ?? Umbraco with serilog and app insight (Open telemetry?)


