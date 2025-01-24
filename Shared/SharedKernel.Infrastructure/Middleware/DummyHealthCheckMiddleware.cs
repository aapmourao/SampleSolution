using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

using SharedKernel.Infrastructure.Common.Settings;

namespace SharedKernel.Infrastructure.Middleware;

public class DummyHealthCheckMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<DummyHealthCheckMiddleware> _logger;
    private readonly HostedServicesSelectorOptions _options;

    public DummyHealthCheckMiddleware(
        RequestDelegate next,
        HostedServicesSelectorOptions options,
        ILogger<DummyHealthCheckMiddleware> logger)
    {
        _next = next;
        _options = options;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.Request.Path.Equals("/health", StringComparison.OrdinalIgnoreCase))
        {
            try
            {
                await context.Response.WriteAsync($"Health Checks: ");
                await context.Response.WriteAsync($" - <a href='db' target='_self'>Database</a>");
                if (_options.ConsumeIntegrationEventsBackgroundService || _options.PublishIntegrationEventsBackgroundService)
                    await context.Response.WriteAsync($" - <a href='rabbitmq' target='_self'>Rabbit Mq</a>");
                else
                    await context.Response.WriteAsync($" - Rabbit Mq (not available)");

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