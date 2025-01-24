using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SharedKernel.Infrastructure.IntegrationEvents.IntegrationEventsPublisher;
using SharedKernel.Infrastructure.IntegrationEvents.MessageBroker;
using SharedKernel.Infrastructure.Persistence.Repositories;
using SharedKernel.IntegrationEvents;

namespace SharedKernel.Infrastructure.IntegrationEvents.BackgroundService;

public class PublishIntegrationEventsBackgroundService(
        IIntegrationEventsPublisher integrationEventPublisher,
        IServiceScopeFactory serviceScopeFactory,
        IOptions<PublishIntegrationEventsOptions> settings,
        ILogger<PublishIntegrationEventsBackgroundService> logger) : IHostedService
{
    private DateOnly lastDayHistoryWasCleaned = DateOnly.FromDateTime(DateTime.UtcNow.Date.AddDays(-1));
    private readonly TimeSpan _initialDelay = TimeSpan.FromSeconds(settings.Value.ServiceOptions.InitialDelayInSeconds); // Delay start on first run
    private readonly TimeSpan _waitForNextTick = TimeSpan.FromSeconds(settings.Value.ServiceOptions.WaitForNextTickInSeconds); // Delay start of each service iteration
    private Task? _doWorkTask = null;
    private PeriodicTimer? _timer = null!;
    private readonly CancellationTokenSource _cts = new CancellationTokenSource();

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _ = Task.Run(async () =>
        {
            try
            {
                var timer = new PeriodicTimer(_initialDelay);
                while (await timer.WaitForNextTickAsync(cancellationToken))
                {
                    _doWorkTask = DoWorkAsync();
                    break;
                }
            }
            catch (OperationCanceledException)
            {
                // Handle the cancellation
            }
        }, cancellationToken);

        return Task.CompletedTask;
    }

    private async Task DoWorkAsync()
    {
        logger.LogInformation("Starting integration event publisher background service.");

        _timer = new PeriodicTimer(_waitForNextTick);

        while (await _timer.WaitForNextTickAsync(_cts.Token))
        {
            try
            {
                await PublishIntegrationEventsFromDbAsync();
            }
            catch (Exception e)
            {
                logger.LogError(e, "Exception occurred while publishing integration events.");
            }
        }
    }

    private async Task PublishIntegrationEventsFromDbAsync()
    {
        using var scope = serviceScopeFactory.CreateScope();
        var cancellationToken = new CancellationToken();
        var outboxIntegrationEventRepository = scope.ServiceProvider.GetRequiredService<IOutboxIntegrationEventRepository>();
        var outboxIntegrationEvents = await outboxIntegrationEventRepository.GetNotProcessedAsync(settings.Value.BatchSize, cancellationToken);

        logger.LogInformation("Read a total of {NumEvents} outbox integration events", outboxIntegrationEvents.Count);

        outboxIntegrationEvents.ForEach(outboxIntegrationEvent => PublishMessage(outboxIntegrationEvent));
 
        if (settings.Value.KeepOutboxRecordsEnabled) {
            outboxIntegrationEventRepository.UpdateRange(outboxIntegrationEvents);
            await outboxIntegrationEventRepository.SaveChangesAsync(cancellationToken);
            if (DateOnly.FromDateTime(DateTime.UtcNow.Date).CompareTo(lastDayHistoryWasCleaned) > 0){
                await outboxIntegrationEventRepository.RemoveHistoryAsync(settings.Value.KeepOutboxRecordsInDays, cancellationToken);
                lastDayHistoryWasCleaned = DateOnly.FromDateTime(DateTime.UtcNow.Date);
            }
        }
        else 
            await outboxIntegrationEventRepository.RemoveRangeAsync(outboxIntegrationEvents, cancellationToken);

    }

    private void PublishMessage(OutboxIntegrationEvent outboxIntegrationEvent)
    {
            var integrationEvent = JsonSerializer.Deserialize<IIntegrationEvent>(outboxIntegrationEvent.EventContent);
            if (integrationEvent is null)
            {
                outboxIntegrationEvent.ProcessedWithError("Error deserializing integration event");
                return;
            }

            try
            {
                logger.LogInformation("Publishing event of type: {EventType} with the {CorrelationId}", integrationEvent.GetType().Name, outboxIntegrationEvent.CorrelationId);
                integrationEventPublisher.PublishEvent(integrationEvent, outboxIntegrationEvent.CorrelationId.ToString());
                logger.LogInformation("Publishing event of type: {EventType} published successfully", integrationEvent.GetType().Name);

                outboxIntegrationEvent.ProcessedWithSuccess("Integration event published successfully");
            }
            catch (Exception e)
            {
                logger.LogError(e, "Publishing exception integration event");
                logger.LogInformation("Publishing event of type: {EventType} thrown an exception", integrationEvent.GetType().Name);
                outboxIntegrationEvent.ProcessedWithError("Error publishing integration event: " + e.Message);
            }
    }


    public async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_doWorkTask is null)
        {
            return;
        }

        await _cts.CancelAsync();
        try
        {
#pragma warning disable VSTHRD003 // Avoid awaiting foreign Tasks
            await _doWorkTask.ConfigureAwait(false);
#pragma warning restore VSTHRD003 // Avoid awaiting foreign Tasks
        }
        catch (OperationCanceledException ex)
        {
            // Handle the cancellation exception if needed
            logger.LogInformation(ex, "Operation cancelled.");
        }

        _timer?.Dispose();
        _cts.Dispose();
    }
}