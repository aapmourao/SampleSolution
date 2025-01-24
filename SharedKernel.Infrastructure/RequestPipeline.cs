using Microsoft.AspNetCore.Builder;
using SharedKernel.Infrastructure.Middleware;

namespace SharedKernel.Infrastructure;

public static class RequestPipeline
{
    public static IApplicationBuilder AddSharedKernelInfrastructureMiddleware(this IApplicationBuilder app)
    {
        app.UseMiddleware<DomainEnumMiddleware>();
        return app;
    }
}