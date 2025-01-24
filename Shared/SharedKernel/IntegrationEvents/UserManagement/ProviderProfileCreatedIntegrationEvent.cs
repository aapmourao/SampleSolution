namespace SharedKernel.IntegrationEvents.UserManagement;

public record ProviderProfileCreatedIntegrationEvent(Guid UserId, Guid SubscriptionAdminId) : IIntegrationEvent;