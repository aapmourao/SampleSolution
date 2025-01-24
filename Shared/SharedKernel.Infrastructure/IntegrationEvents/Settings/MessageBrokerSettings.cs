using SharedKernel.Infrastructure.IntegrationEvents.MessageBroker;

namespace SharedKernel.Infrastructure.IntegrationEvents.Settings;

public class MessageBrokerSettings
{
    public const string Section = "MessageBroker";
    public string ClientType { get; init; } = MessageBrokerTypes.RabbitMQ.ToString();

    public MessageBrokerSettingsConnection Connection { get; init; } = null!;
    public List<MessageBrokerSettingsExchange> Exchanges { get; init; } = null!;
}

public class MessageBrokerSettingsExchange
{
    public string Name { get; init; } = null!;
    public string Type { get; init; } = "Direct";
    public bool Durable { get; init; } = true;

    public List<MessageBrokerSettingsQueue> Queues { get; init; } = default!;

}
public class MessageBrokerSettingsQueue
{
    public string Name { get; init; } = null!;
    public string RoutingKey { get; init; } = string.Empty;
    public bool Durable { get; init; } = true;
    public bool Exclusive { get; init; } = false;
    public bool AutoDelete { get; init; } = false;
    public string Arguments { get; init; } = string.Empty;
}

public class MessageBrokerSettingsConnection
{
    public string HostName { get; init; } = null!;
    public int Port { get; init; }
    public string UserName { get; init; } = null!;
    public string Password { get; init; } = null!;
    public string VirtualHost { get; init; } = null!;
}