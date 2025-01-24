namespace SharedKernel.Infrastructure.IntegrationEvents.MessageBroker;

    public class MessageBrokerConsumerSettings
    {
        public static string Section = "MessageBrokerConsumer";
        public List<string> Queues { get; init; } = null!;
        public BackgroundServiceOptions ServiceOptions { get; init; } = null!;
    }