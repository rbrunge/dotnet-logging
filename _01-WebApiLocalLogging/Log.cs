// ReSharper disable once CheckNamespace
namespace CommonLogging;

public static partial class Log
{
    [LoggerMessage(
        EventId = 0,
        Level = LogLevel.Critical,
        Message = "This is an example of source generated error using [LoggerMessage] .. custom message: `{message}`")]
    public static partial void CustomSourceGeneratedLogMessage(
        this ILogger logger, string message);
}