using System.Text.Json.Serialization;

using MediatR;

using SharedKernel.IntegrationEvents.SubscriptionManagement;
using SharedKernel.IntegrationEvents.UserManagement;
using SharedKernel.IntegrationEvents.SessionManagement;

namespace SharedKernel.IntegrationEvents;

[JsonDerivedType(typeof(AdminProfileCreatedIntegrationEvent), typeDiscriminator: nameof(AdminProfileCreatedIntegrationEvent))]
[JsonDerivedType(typeof(ParticipantProfileCreatedIntegrationEvent), typeDiscriminator: nameof(ParticipantProfileCreatedIntegrationEvent))]
[JsonDerivedType(typeof(ProviderProfileCreatedIntegrationEvent), typeDiscriminator: nameof(ProviderProfileCreatedIntegrationEvent))]
[JsonDerivedType(typeof(AdminProfileRemovedIntegrationEvent), typeDiscriminator: nameof(AdminProfileRemovedIntegrationEvent))]
[JsonDerivedType(typeof(ParticipantProfileRemovedIntegrationEvent), typeDiscriminator: nameof(ParticipantProfileRemovedIntegrationEvent))]
[JsonDerivedType(typeof(ProviderProfileRemovedIntegrationEvent), typeDiscriminator: nameof(ProviderProfileRemovedIntegrationEvent))]
[JsonDerivedType(typeof(PlaceAddedIntegrationEvent), typeDiscriminator: nameof(PlaceAddedIntegrationEvent))]
[JsonDerivedType(typeof(PlaceRemovedIntegrationEvent), typeDiscriminator: nameof(PlaceRemovedIntegrationEvent))]
[JsonDerivedType(typeof(SessionScheduledIntegrationEvent), typeDiscriminator: nameof(SessionScheduledIntegrationEvent))]
public interface IIntegrationEvent : INotification { }