using SharedKernel.Domain.Common;

namespace SharedKernel.Domain.Core;

public interface IAggregateRoot
{
    string GetEntityIdValue();
    List<IDomainEvent> PopDomainEvents();
}