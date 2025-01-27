using Microsoft.AspNetCore.Mvc;

namespace WebApiLocalLogging;

public static class LoggingApi
{
    public static WebApplication AddLoggingApi(this WebApplication app)
    {
        var group = app.MapGroup("/api/test");

        group.MapGet("/log-all", ([FromServices] ILogger<Program> logger) =>
        {
            var dt = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss");
            logger.LogDebug($"LogDebug from TestController at {dt}");
            logger.LogTrace($"LogTrace from TestController at {dt}");
            logger.LogInformation($"LogInformation from TestController at {dt}");
            logger.LogWarning($"LogWarning from TestController at {dt}");
            logger.LogError($"LogError from TestController at {dt}");
            logger.LogCritical($"LogCritical from TestController at {dt}");

            return Results.Ok("Logged all log levels");
        });
            //.WithName("GetWeatherForecast");
        
        return app;
    }
}