using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SharedKernel.Infrastructure.IntegrationEvents.MessageBroker;

using Throw;

namespace SharedKernel.Infrastructure.IntegrationEvents.BackgroundService;

public class ConsumeIntegrationEventsBackgroundService : IHostedService
{
    private readonly List<ConsumeIntegrationEvent> _consumeIntegrationEvents = new List<ConsumeIntegrationEvent>();
    private readonly ILogger<ConsumeIntegrationEventsBackgroundService> _logger;
    private readonly CancellationTokenSource _cts;
    private readonly TimeSpan _initialDelay; // Delay start on first run
    private TimeSpan _waitForNextTick; // Delay start of each service iteration
    public ConsumeIntegrationEventsBackgroundService(
        ILogger<ConsumeIntegrationEventsBackgroundService> logger,
        IServiceScopeFactory serviceScopeFactory,
        IMessageBrokerFactory messageBrokerFactory,
        IOptions<MessageBrokerConsumerSettings> settings)
    {
        _logger = logger;
        _cts = new CancellationTokenSource();

        foreach (var queue in settings.Value.Queues)
        {
            _consumeIntegrationEvents.Add(new ConsumeIntegrationEvent(messageBrokerFactory.CreateMessageBrokerClient(), queue, serviceScopeFactory, logger, _cts));
        }

        _initialDelay = TimeSpan.FromSeconds(settings.Value.ServiceOptions.InitialDelayInSeconds);
        _waitForNextTick = TimeSpan.FromSeconds(settings.Value.ServiceOptions.WaitForNextTickInSeconds); // Delay start of each service iteration
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Initiating integration event consumer background service.");

        _ = Task.Run(async () =>
        {
            var timer1 = new PeriodicTimer(_initialDelay);
            await timer1.WaitForNextTickAsync(cancellationToken);

            if (cancellationToken.IsCancellationRequested)
                return;

            _logger.LogInformation($"Starting integration event consumer background service.");
            try
            {
                var timer = new PeriodicTimer(_waitForNextTick);
                while (await timer.WaitForNextTickAsync(cancellationToken))
                {
                    if (cancellationToken.IsCancellationRequested)
                        return;

                    foreach (var consumeIntegrationEvent in _consumeIntegrationEvents)
                    {
                        var timer2 = new PeriodicTimer(TimeSpan.FromSeconds(5));
                        await timer2.WaitForNextTickAsync(cancellationToken);
                        _logger.LogInformation($"Starting integration event consumer background service subscribing to queue {consumeIntegrationEvent.QueueName}.");
                        consumeIntegrationEvent.Subscribe();
                    };
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

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await _cts.CancelAsync();
        _cts.Dispose();
    }
}