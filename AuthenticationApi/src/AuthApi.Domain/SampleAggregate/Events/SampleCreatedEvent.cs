using SharedKernel.Domain.Common;

namespace AuthApi.Domain.SampleAggregate.Events;

public record SampleCreatedEvent(Guid SampleId) : IDomainEvent
{
    public Guid? Id { get; set; }
}
