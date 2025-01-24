using CleanTemplate.Infrastructure.Persistence;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SharedKernel.Infrastructure.EventualConsistency.Options;
using SharedKernel.Infrastructure.Middleware;
using SharedKernel.Infrastructure.Persistence.Repositories;

namespace CleanTemplate.Infrastructure.Middleware;

public class EventualConsistencyMiddleware(
    RequestDelegate next,
    IOptions<EventualConsistencyOptions> eventualConsistencyOptions,
    ILogger<EventualConsistencyMiddleware> logger)
{
    public async Task InvokeAsync(
        HttpContext context,
        IPublisher publisher,
        InfrastructureDbContext dbContext,
        IEventualConsistencyMessageRepository eventualConsistencyMessageRepository)
    {
        using var transaction = await dbContext.Database.BeginTransactionAsync();
        await EventualConsistencyHelper.ContextResponseOnCompletedAsync(
            eventualConsistencyOptions,
            context,
            publisher,
            eventualConsistencyMessageRepository,
            transaction);

        dbContext.SaveChanges();
        await transaction.CommitAsync();

        try
        {
            await next(context);
        }
        catch (System.Exception ex)
        {
            logger.LogError(ex, "Error processing Eventual Consistency Middleware.");
        }
    }
}
