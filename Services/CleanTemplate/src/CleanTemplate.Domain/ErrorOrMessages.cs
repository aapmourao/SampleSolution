using Microsoft.Extensions.Logging;

namespace CleanTemplate.Domain;

internal partial class ErrorOrMessages
{
    internal static string CouldNotOpenSocketMessage = "Could not open socket to `{hostName}`";

    [LoggerMessage(
        EventId = 0,
        Level = LogLevel.Information,
        Message = "Could not open socket to `{HostName}`")]
    internal static partial void InfoCouldNotOpenSocket(ILogger logger, string hostName);
}
