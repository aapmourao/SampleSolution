using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Options;
using SharedKernel.Core.EventualConsistency;
using SharedKernel.Domain.Common;
using SharedKernel.Infrastructure.EventualConsistency.Options;
using SharedKernel.Infrastructure.Persistence.Repositories;

namespace SharedKernel.Infrastructure.Middleware;

public static class EventualConsistencyHelper
{
    public static Task ContextResponseOnCompletedAsync(IOptions<EventualConsistencyOptions> eventualConsistencyOptions, HttpContext context, IPublisher publisher, IEventualConsistencyMessageRepository eventualConsistencyMessageRepository, IDbContextTransaction transaction)
    {
        context.Response.OnCompleted(async () =>
        {
            IDomainEvent? lastNextEvent = null;
            try
            {
                if (context.Items.TryGetValue(SharedKernel.Infrastructure.Persistence.BaseDbContext.DomainEventsKey, out var value) && value is Queue<IDomainEvent> domainEvents)
                {
                    while (domainEvents.TryDequeue(out var nextEvent))
                    {
                        lastNextEvent = nextEvent;
                        await publisher.Publish(nextEvent);

                        await updatedEventualConsistencyMessageAsync(
                            eventualConsistencyMessageRepository,
                            nextEvent,
                            eventualConsistencyOptions.Value.KeepHistoryEnabled,
                            eventualConsistencyOptions.Value.KeepHistoryInDays
                        ).ConfigureAwait(false);
                    }
                }

            }
            catch (EventualConsistencyException ex)
            {
                await transaction.DisposeAsync();
                // ToDo: Process eventual Consistency exception
                if (lastNextEvent is { })
                {
                    await updatedEventualConsistencyMessageExceptionAsync(
                        eventualConsistencyMessageRepository,
                        lastNextEvent,
                        $"{ex.EventualConsistencyError.Code} - {ex.EventualConsistencyError.Description}")
                    .ConfigureAwait(false);
                }

                // This line was removed from the original code
                // dbContext.SaveChanges(); 
            }
        });

        return Task.CompletedTask;
    }

    private static async Task updatedEventualConsistencyMessageExceptionAsync(
        IEventualConsistencyMessageRepository eventualConsistencyMessageRepository,
        IDomainEvent nextEvent,
        string ExceptionMessage)
    {
        if (nextEvent.Id is null)
            return;

        var eventualConsistencyMessage = await eventualConsistencyMessageRepository.GetByIdAsync(nextEvent.Id!.Value, CancellationToken.None);

        if (eventualConsistencyMessage is { })
        {
            eventualConsistencyMessage.ProcessedWithError(ExceptionMessage);
            eventualConsistencyMessageRepository.Update(eventualConsistencyMessage);
            await eventualConsistencyMessageRepository.SaveChangesAsync(CancellationToken.None).ConfigureAwait(false);
        }
    }

    private static async Task updatedEventualConsistencyMessageAsync(
        IEventualConsistencyMessageRepository eventualConsistencyMessageRepository,
        IDomainEvent nextEvent,
        bool KeepHistoryEnabled,
        int KeepHistoryInDays)
    {
        if (nextEvent.Id is null)
            return;

        var eventualConsistencyMessage = await eventualConsistencyMessageRepository.GetByIdAsync(nextEvent.Id.Value, CancellationToken.None);

        if (eventualConsistencyMessage is null)
            return;

        if (KeepHistoryEnabled)
        {
            eventualConsistencyMessage.ProcessedWithSuccess("Success!");
            eventualConsistencyMessageRepository.Update(eventualConsistencyMessage);
            await eventualConsistencyMessageRepository.SaveChangesAsync(CancellationToken.None).ConfigureAwait(false);

            await eventualConsistencyMessageRepository.RemoveHistoryAsync(KeepHistoryInDays, CancellationToken.None).ConfigureAwait(false);
        }
        else
        {
            eventualConsistencyMessageRepository.Remove(eventualConsistencyMessage);
            await eventualConsistencyMessageRepository.SaveChangesAsync(CancellationToken.None).ConfigureAwait(false);
        }
    }
}
