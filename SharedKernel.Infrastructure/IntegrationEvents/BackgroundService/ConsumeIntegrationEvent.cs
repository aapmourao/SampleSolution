using System.Text.Json;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client.Events;
using SharedKernel.Infrastructure.IntegrationEvents.MessageBroker;
using SharedKernel.IntegrationEvents;

namespace SharedKernel.Infrastructure.IntegrationEvents.BackgroundService;

public class ConsumeIntegrationEvent(
    IMessageBrokerClient messageBrokerClient,
    string queueName,
    IServiceScopeFactory serviceScopeFactory,
    ILogger _logger,
    CancellationTokenSource cancellationToken)
{
    public string QueueName { get => queueName; }

    public void Subscribe()
    {
        _logger.LogDebug(">>> ConsumeIntegrationEvent: Subscribe");
        messageBrokerClient.Subscribe(queueName, ConsumeIntegrationEventCaller);
        _logger.LogDebug(">>> ConsumeIntegrationEvent: Subscribe");
    }
#pragma warning disable VSTHRD100 // Avoid async void methods
    private async void ConsumeIntegrationEventCaller(
#pragma warning restore VSTHRD100 // Avoid async void methods
        string message,
        BasicDeliverEventArgs eventArgs)
    {
        _logger.LogDebug(">>> ConsumeIntegrationEventCaller {message}", message);
        if (cancellationToken.IsCancellationRequested)
        {
            _logger.LogInformation("Cancellation requested, not consuming integration event.");
            _logger.LogDebug("<<< ConsumeIntegrationEventCaller");
            return;
        }

        try
        {
            _logger.LogInformation("Received integration event. Reading message from queue.");

            using var scope = serviceScopeFactory.CreateScope();

            var publisher = scope.ServiceProvider.GetRequiredService<IPublisher>();
            var inboxIntegrationEvent = GetInboxIntegrationEvent(eventArgs);

            var integrationEvent = JsonSerializer.Deserialize<IIntegrationEvent>(message);
            if (integrationEvent is null)
            {
                _logger.LogError("Error deserializing integration event message. Message will be Dead-Lettered.");
                messageBrokerClient.DeadLetter(eventArgs);

                _logger.LogInformation("Publish Inbox Integration event.");
                inboxIntegrationEvent.ProcessedWithError($"Error deserializing integration event message. Message will be Dead-Lettered. {message}");
                await publisher.Publish(inboxIntegrationEvent);

                _logger.LogDebug("<<< ConsumeIntegrationEventCaller");
                return;
            }

            try
            {
                _logger.LogInformation(
                    "Received integration event of type: {IntegrationEventType}. Publishing event.",
                    integrationEvent.GetType().Name);

                await publisher.Publish(integrationEvent);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error validating integration event. Message will be Dead-Lettered.");
                messageBrokerClient.DeadLetter(eventArgs);

                _logger.LogInformation("Publish Inbox Integration event.");
                inboxIntegrationEvent.ProcessedWithError($"Message {integrationEvent.GetType()} will be Dead-lettered! Exception: {e.Message}");
                await publisher.Publish(inboxIntegrationEvent);

                _logger.LogDebug("<<< ConsumeIntegrationEventCaller");
                return;
            }

            _logger.LogInformation("Publish Inbox Integration event.");
            inboxIntegrationEvent.ProcessedWithSuccess($"{integrationEvent.GetType()} event published successfully");
            await publisher.Publish(inboxIntegrationEvent);

            _logger.LogInformation("Integration event published in Location Management service successfully. Sending ack to message broker.");

            messageBrokerClient.Success(eventArgs);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Exception occurred while consuming integration event");
        }
        _logger.LogDebug("<<< ConsumeIntegrationEventCaller");
    }

    private static InboxIntegrationEvent GetInboxIntegrationEvent(BasicDeliverEventArgs eventArgs)
    {
        return new InboxIntegrationEvent
        (
            Publisher: eventArgs.BasicProperties.Headers["publisher"] is byte[] publisherBytes ? System.Text.Encoding.UTF8.GetString(publisherBytes) : string.Empty,
            MessageType: eventArgs.BasicProperties.Headers["messageType"] is byte[] messageTypeBytes ? System.Text.Encoding.UTF8.GetString(messageTypeBytes) : string.Empty,
            CorrelationId: eventArgs.BasicProperties.Headers["correlationId"] is byte[] correlationIdBytes && Guid.TryParse(System.Text.Encoding.UTF8.GetString(correlationIdBytes), out var correlationId) ? correlationId : Guid.Empty,
            TimeStamp: eventArgs.BasicProperties.Headers["timestamp"] is byte[] timestampBytes ? System.Text.Encoding.UTF8.GetString(timestampBytes) : string.Empty
        );
    }
}