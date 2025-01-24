namespace SharedKernel.IntegrationEvents.UserManagement;

public record AdminProfileRemovedIntegrationEvent(Guid UserId, Guid AdminId) : IIntegrationEvent;