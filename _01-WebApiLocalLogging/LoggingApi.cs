using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

// ReSharper disable once CheckNamespace
namespace CommonLogging;

public static class LoggingApi
{
    public static WebApplication AddLoggingApi(this WebApplication app)
    {
        var group = app.MapGroup("/api/test");
        var executingAssemblyName = System.Reflection.Assembly.GetEntryAssembly()?.GetName().Name;
        string MessagePostFix() => $"at {DateTime.UtcNow:yyyy-MM-ddTHH:mm:ss} ({executingAssemblyName})";

        group.MapGet("/log-all", ([FromServices] ILogger<IAppMarker> logger) =>
        {
            var myStructuredData = JsonSerializer.Serialize(new MyRecord(MessagePostFix()));
            logger.LogDebug("LogDebug {MyStructuredData}", myStructuredData);
            logger.LogTrace("LogTrace {MyStructuredData}", myStructuredData);
            logger.LogInformation("LogInformation {MyStructuredData}", myStructuredData);
            logger.LogWarning("LogWarning {MyStructuredData}", myStructuredData);
            logger.LogError("LogError {MyStructuredData}", myStructuredData);
            logger.LogCritical("LogCritical {MyStructuredData}", myStructuredData);
            
            return Results.Ok("Log messages sent");
        });

        group.MapGet("/throw",
            ([FromServices] ILogger<IAppMarker> logger) =>
            {
                throw new CustomException("Testing throw with no handling, " + MessagePostFix());
            });

        group.MapGet("/throw-try-catch", ([FromServices] ILogger<IAppMarker> logger) =>
        {
            try
            {
                throw new CustomException("Testing throw with try catch, " + MessagePostFix());
            }
            catch (CustomException e)
            {
                logger.LogError(e, e.Message);
            }
        });

        group.MapPost("/source-generated", async (
            [FromServices] ILogger<IAppMarker> logger,
            HttpRequest request) =>
        {
            using var reader = new StreamReader(request.Body);
            var body = await reader.ReadToEndAsync()
                       ?? MessagePostFix();

            logger.CustomSourceGeneratedLogMessage(body );

            return body;
        });

        return app;
    }

    internal record MyRecord(string Message)
    {
        public DateTime TimeStamp { get; } = DateTime.UtcNow;

        public Dictionary<string, object> MyDictionary { get; } = new()
        {
            { "key1", "value1" },
            { "key2", 2 },
            { "key3", new { subKey1 = "subValue1" } }
        };
        
        public List<string> MyList { get; } = ["item1", "item2"];
    }
}