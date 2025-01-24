using CleanTemplate.Infrastructure.Middleware;
using Microsoft.AspNetCore.Builder;
using SharedKernel.Infrastructure;

namespace CleanTemplate.Infrastructure;

public static class RequestPipeline
{
    public static IApplicationBuilder AddInfrastructureMiddleware(this IApplicationBuilder app)
    {
        app.UseMiddleware<EventualConsistencyMiddleware>();
        app.AddSharedKernelInfrastructureMiddleware();
        return app;
    }
}
