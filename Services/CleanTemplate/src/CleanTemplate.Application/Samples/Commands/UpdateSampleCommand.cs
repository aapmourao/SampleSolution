using CleanTemplate.Application.Common.Interfaces;
using CleanTemplate.Domain.SampleAggregate;
using SharedKernel.Domain.Core;

namespace CleanTemplate.Application.Samples.Commands;

public record UpdateSampleCommand(Guid Id, string? Name, string? Description) : IRequest<ErrorOr<Sample>>;

public class UpdateSampleCommandHandler(ISampleRepository repository) : IRequestHandler<UpdateSampleCommand, ErrorOr<Sample>>
{
    public async Task<ErrorOr<Sample>> Handle(UpdateSampleCommand command, CancellationToken cancellationToken)
    {
        var sample = await repository.GetByIdAsync(command.Id, cancellationToken);
        if (sample is null)
        {
            return Error.Failure("Sample not found");
        }

        sample.Update(command.Name, command.Description);
        sample.AddEntityId(EntityId.CreateUnique());

        var updatedSample = await repository.UpdateAsync(sample, cancellationToken);
        return updatedSample;
    }
}
