namespace SharedKernel.Infrastructure.IntegrationEvents
{
    public class BackgroundServiceOptions
    {
        public int InitialDelayInSeconds { get; init; }
        public int WaitForNextTickInSeconds { get; init; }
    }
}