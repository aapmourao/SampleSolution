using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using SharedKernel.Infrastructure.Middleware;

namespace SharedKernel.Infrastructure;

public static class RequestPipeline
{
    public static IApplicationBuilder AddSharedKernelInfrastructureMiddleware(this IApplicationBuilder app)
    {
        app.UseMiddleware<DomainEnumMiddleware>();
        // Health check middleware
        var configuration = app.ApplicationServices.GetService<IConfiguration>();

        app.UseMiddleware<DbHealthCheckMiddleware>();
        app.UseMiddleware<RootHealthCheckMiddleware>();
        if (configuration is not null
            && (configuration.GetValue<bool>("HostedServicesSelector:PublishIntegrationEventsBackgroundService")
            || configuration.GetValue<bool>("HealthChecks:HostedServicesSelector:PublishIntegrationEventsBackgroundService")))
        {
            app.UseMiddleware<RabbitMqHealthCheckMiddleware>();
        }

        return app;
    }
}