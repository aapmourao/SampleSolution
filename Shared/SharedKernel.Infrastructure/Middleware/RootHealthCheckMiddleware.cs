using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using SharedKernel.Infrastructure.Common.Settings;

namespace SharedKernel.Infrastructure.Middleware;

public class RootHealthCheckMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RootHealthCheckMiddleware> _logger;
    private readonly HostedServicesSelectorOptions _options;

    public RootHealthCheckMiddleware(
        RequestDelegate next,
        IOptions<HostedServicesSelectorOptions> options,
        ILogger<RootHealthCheckMiddleware> logger)
    {
        _next = next;
        _options = options.Value;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.Request.Path.Equals("/health", StringComparison.OrdinalIgnoreCase))
        {
            try
            {
                context.Response.ContentType = "text/html";
                await context.Response.WriteAsync($"<html><body>Health Checks: ");
                await context.Response.WriteAsync($"<ul><li><a href='health/db' target='_self'>Database</a></li>");
                if (_options.ConsumeIntegrationEventsBackgroundService || _options.PublishIntegrationEventsBackgroundService)
                    await context.Response.WriteAsync($"<li><a href='health/rabbitmq' target='_self'>Rabbit Mq</a></li>");
                else
                    await context.Response.WriteAsync($"<li>Rabbit Mq (not available)</li>");

                await context.Response.WriteAsync($"</ul></body></html>");

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Setup health checks has failed.");
                context.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
            }

            return;
        }

        await _next(context);
    }
}