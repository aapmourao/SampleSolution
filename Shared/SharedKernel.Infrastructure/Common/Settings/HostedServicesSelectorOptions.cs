namespace SharedKernel.Infrastructure.Common.Settings;
public class HostedServicesSelectorOptions
{
    public static string Section = "HostedServicesSelector";

    public bool IntegrationEventsPublisher { get; set; }
    public bool PublishIntegrationEventsBackgroundService { get; set; }
    public bool ConsumeIntegrationEventsBackgroundService { get; set; }
}