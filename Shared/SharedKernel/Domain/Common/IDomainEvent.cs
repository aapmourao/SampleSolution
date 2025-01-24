using MediatR;

namespace SharedKernel.Domain.Common;

public interface IDomainEvent : INotification
{
    Guid? Id { get; set; }
}