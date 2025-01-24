namespace SharedKernel.Infrastructure.IntegrationEvents.MessageBroker;

public class MessageBrokerPublisherSettings
{
    public static string Section = "MessageBrokerPublisher";

    public string Publisher { get; init; } = null!;
    public string ExchangeName { get; init; } = null!;

}