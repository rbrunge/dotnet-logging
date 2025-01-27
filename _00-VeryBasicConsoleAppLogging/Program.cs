using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var builder = Host.CreateApplicationBuilder(args);

// The line below controls the loggerAfter but not the loggerBefore or anotherLoggerBefore
builder.Logging.AddConsole()
    //.SetMinimumLevel(LogLevel.Warning)
    ;

using var loggerFactory = LoggerFactory.Create(loggingBuilder =>
{
    loggingBuilder
        .AddConsole()
        .SetMinimumLevel(LogLevel.Debug);
});

using var anotherLoggerFactory = LoggerFactory.Create(loggingBuilder =>
{
    loggingBuilder
        .AddConsole()
        .SetMinimumLevel(LogLevel.Trace);
});

var loggerBefore = loggerFactory.CreateLogger<Program>();
loggerBefore.LogTrace("LogTrace before host build (should not be shown)");
loggerBefore.LogDebug("LogDebug before host build");
loggerBefore.LogInformation("LogInformation before host build");

var anotherLoggerBefore = anotherLoggerFactory.CreateLogger<Program>();
anotherLoggerBefore.LogTrace("Another LogTrace before host build");
anotherLoggerBefore.LogDebug("Another LogDebug before host build");
anotherLoggerBefore.LogInformation("Another LogInformation before host build");

using IHost host = builder.Build();

var loggerAfter = host.Services.GetRequiredService<ILogger<Program>>();
loggerAfter.LogTrace("LogTrace right after host build (should not be shown)");
loggerAfter.LogDebug("LogDebug right after host build (should not be shown)");
loggerAfter.LogInformation("LogInformation right after host build");

Console.WriteLine("Hello, World!");


await host.RunAsync();
