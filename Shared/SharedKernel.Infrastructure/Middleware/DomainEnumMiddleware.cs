using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Infrastructure.Persistence.Repositories;

namespace SharedKernel.Infrastructure.Middleware;

public class DomainEnumMiddleware(
    RequestDelegate next)
{
    public async Task InvokeAsync(
        HttpContext context, IDomainEnumDbContext dbContext
        )
    {
        if (context.Request.Method == HttpMethod.Get.Method
        && context.Request.Path.Equals("/shared/domainenums", StringComparison.OrdinalIgnoreCase))
        {
            var categories = await dbContext.DomainEnums.ToListAsync();
            var jsonResponse = JsonSerializer.Serialize(categories);
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(jsonResponse);
            return;
        }

        await next(context);
    }
}