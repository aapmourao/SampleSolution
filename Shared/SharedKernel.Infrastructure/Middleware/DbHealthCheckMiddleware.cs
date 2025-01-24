using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using SharedKernel.Infrastructure.Persistence.Repositories;

namespace SharedKernel.Infrastructure.Middleware;

public class DbHealthCheckMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<DbHealthCheckMiddleware> _logger;

    public DbHealthCheckMiddleware(RequestDelegate next, ILogger<DbHealthCheckMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, IDomainEnumDbContext dbContext)
    {
        if (context.Request.Path.Equals("/health/db", StringComparison.OrdinalIgnoreCase))
        {
            try
            {
                await dbContext.Database.CanConnectAsync();
                context.Response.StatusCode = StatusCodes.Status200OK;
                await context.Response.WriteAsync("Database is healthy");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Database health check failed.");
                context.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
                await context.Response.WriteAsync("Database is unhealthy");
            }

            return;
        }

        await _next(context);
    }
}