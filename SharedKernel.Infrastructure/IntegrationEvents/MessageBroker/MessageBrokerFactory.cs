using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using SharedKernel.Infrastructure.IntegrationEvents.Settings;

namespace SharedKernel.Infrastructure.IntegrationEvents.MessageBroker;

public interface IMessageBrokerFactory
{
    IMessageBrokerClient CreateMessageBrokerClient();
}

public class MessageBrokerFactory(
    IServiceScopeFactory serviceScopeFactory
): IMessageBrokerFactory
{
    public IMessageBrokerClient CreateMessageBrokerClient()
    {
        var messageBrokerType = MessageBrokerTypes.RabbitMQ;
        using var scope = serviceScopeFactory.CreateScope();
        return messageBrokerType switch
        {
            MessageBrokerTypes.RabbitMQ => scope.ServiceProvider.GetRequiredService<RabbitMQClient>(),
            _ => throw new ArgumentException($"Message broker type {messageBrokerType} not supported")
        };
    }
}