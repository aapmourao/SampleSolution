using System.Text;
using System.Text.Json;

using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using SharedKernel.IntegrationEvents;
using SharedKernel.Infrastructure.IntegrationEvents.Settings;
using System.Reflection;
using SharedKernel.Infrastructure.IntegrationEvents.MessageBroker;

namespace SharedKernel.Infrastructure.IntegrationEvents.IntegrationEventsPublisher;

public class IntegrationEventsPublisher : IIntegrationEventsPublisher
{
    private readonly string _exchangeName;

    private IMessageBrokerClient messageBrokerClient;

    public IntegrationEventsPublisher(
        IOptions<MessageBrokerPublisherSettings> messageBrokerPublisherSettings,
        IMessageBrokerFactory messageBrokerFactory)
    {
        _exchangeName = messageBrokerPublisherSettings.Value.ExchangeName;
        messageBrokerClient = messageBrokerFactory.CreateMessageBrokerClient();
    }

    public void PublishEvent(IIntegrationEvent integrationEvent, string correlationId)
    {
        var eventTypeName = integrationEvent.GetType().FullName?.Split('.')[^1];
        string serializedIntegrationEvent = JsonSerializer.Serialize(integrationEvent);

        byte[] body = Encoding.UTF8.GetBytes(serializedIntegrationEvent);

        messageBrokerClient.Publish(
            exchangeName: _exchangeName,
            eventTypeName: eventTypeName!,
            correlationId: correlationId,
            body: body
            );
    }
}