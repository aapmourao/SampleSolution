namespace SharedKernel.IntegrationEvents.UserManagement;

public record ParticipantProfileRemovedIntegrationEvent(Guid UserId, Guid SubscriptionAdminId) : IIntegrationEvent;