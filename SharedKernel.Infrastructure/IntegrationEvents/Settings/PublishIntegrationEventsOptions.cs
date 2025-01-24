namespace SharedKernel.Infrastructure.IntegrationEvents.MessageBroker;
    public class PublishIntegrationEventsOptions
    {
        public static string Section = "PublishIntegrationEvents";
        public bool KeepOutboxRecordsEnabled { get; init; } = false;
        public int KeepOutboxRecordsInDays { get; init; } = 5;
        public int BatchSize { get; init; } = 100;
        public BackgroundServiceOptions ServiceOptions { get; init; } = null!;
    }
