using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using SharedKernel.Infrastructure.Common.Settings;

namespace SharedKernel.Infrastructure.Middleware;

public class RabbitMqHealthCheckMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RabbitMqHealthCheckMiddleware> _logger;
    private readonly ConnectionFactory _connectionFactory;

    public RabbitMqHealthCheckMiddleware(RequestDelegate next, ILogger<RabbitMqHealthCheckMiddleware> logger, ConnectionFactory connectionFactory)
    {
        _next = next;
        _logger = logger;
        _connectionFactory = connectionFactory;
    }

    public async Task InvokeAsync(HttpContext context, HostedServicesSelectorOptions options)
    {
        if (context.Request.Path.Equals("/health/rabbitmq", StringComparison.OrdinalIgnoreCase))
        {
            var isEnabled = "Enabled";
            var isDisabled = "Disabled";
            var status = options.ConsumeIntegrationEventsBackgroundService ? isEnabled : isDisabled;
            await context.Response.WriteAsync($"Consume Integration services is {status}");
            status = options.PublishIntegrationEventsBackgroundService ? isEnabled : isDisabled;
            await context.Response.WriteAsync($"Publish Integration services is {status}");

            try
            {
                using (var connection = _connectionFactory.CreateConnection())
                using (var channel = connection.CreateModel())
                {
                    context.Response.StatusCode = StatusCodes.Status200OK;
                    await context.Response.WriteAsync("RabbitMQ is healthy");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "RabbitMQ health check failed.");
                context.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
                await context.Response.WriteAsync("RabbitMQ is unhealthy");
            }

            return;
        }

        await _next(context);
    }
}