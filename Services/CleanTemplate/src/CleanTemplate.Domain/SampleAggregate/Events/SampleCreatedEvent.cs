using SharedKernel.Domain.Common;

namespace CleanTemplate.Domain.SampleAggregate.Events;

public record SampleCreatedEvent(Guid SampleId) : IDomainEvent
{
    public Guid? Id { get; set; }
}
