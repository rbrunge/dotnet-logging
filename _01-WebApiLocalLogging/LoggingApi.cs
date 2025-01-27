using Microsoft.AspNetCore.Mvc;

// ReSharper disable once CheckNamespace
namespace CommonLogging;

public static class LoggingApi
{
    public static WebApplication AddLoggingApi(this WebApplication app)
    {
        var group = app.MapGroup("/api/test");

        group.MapGet("/log-all", ([FromServices] ILogger<IAppMarker> logger) =>
        {
            var dt = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss");
            logger.LogDebug($"LogDebug from TestController at {dt}");
            logger.LogTrace($"LogTrace from TestController at {dt}");
            logger.LogInformation($"LogInformation from TestController at {dt}");
            logger.LogWarning($"LogWarning from TestController at {dt}");
            logger.LogError($"LogError from TestController at {dt}");
            logger.LogCritical($"LogCritical from TestController at {dt}");

            return Results.Ok("Log messages sent");
        });

        group.MapGet("/throw",
            ([FromServices] ILogger<IAppMarker> logger) =>
            {
                throw new CustomException("Testing throw with no handling");
            });

        group.MapGet("/throw-try-catch", ([FromServices] ILogger<IAppMarker> logger) =>
        {
            try
            {
                throw new CustomException("Testing throw with try catch");
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
                       ?? "Message is empty - This is a test ...";

            logger.CustomSourceGeneratedLogMessage(body );

            return body;
        });

        return app;
    }
}