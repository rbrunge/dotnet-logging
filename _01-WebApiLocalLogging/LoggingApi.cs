using Microsoft.AspNetCore.Mvc;

// ReSharper disable once CheckNamespace
namespace CommonLogging;

public static class LoggingApi
{
    public static WebApplication AddLoggingApi(this WebApplication app)
    {
        var group = app.MapGroup("/api/test");
        var executingAssemblyName = System.Reflection.Assembly.GetEntryAssembly()?.GetName().Name;
        string MessagePostFix() => $"from TestController {DateTime.UtcNow:yyyy-MM-ddTHH:mm:ss} ({executingAssemblyName})";

        group.MapGet("/log-all", ([FromServices] ILogger<IAppMarker> logger) =>
        {
            logger.LogDebug("LogDebug {messagePostFix}", MessagePostFix());
            logger.LogTrace("LogTrace {messagePostFix}", MessagePostFix());
            logger.LogInformation("LogInformation {messagePostFix}", MessagePostFix());
            logger.LogWarning("LogWarning {messagePostFix}", MessagePostFix());
            logger.LogError("LogError {messagePostFix}", MessagePostFix());
            logger.LogCritical("LogCritical {messagePostFix}", MessagePostFix());

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
}