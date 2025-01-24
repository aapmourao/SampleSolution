using SharedKernel.Infrastructure.Common.Model;

namespace SharedKernel.Infrastructure.EventualConsistency;

public record EventualConsistencyMessage(
    string EventName, 
    string EventContent, 
    Guid CorrelationId, 
    string CorrelationEntity): TrackingProcessedStatus {}