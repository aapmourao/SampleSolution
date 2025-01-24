using SharedKernel.Domain.Common;
using SharedKernel.Infrastructure.Common.Model;

namespace SharedKernel.Infrastructure.IntegrationEvents;

public record InboxIntegrationEvent(
    string Publisher,
    string MessageType,
    string TimeStamp,
        Guid CorrelationId = default) : TrackingProcessedStatus, IDomainEvent
    {
        public new Guid? Id { get; set; } = Guid.NewGuid();
    }