using CleanTemplate.Application.Common.Interfaces;
using CleanTemplate.Domain.SampleAggregate;
using SharedKernel.Domain.Core;

namespace CleanTemplate.Application.Samples.Commands;

public record AddSampleCommand(string Name, string Description) : IRequest<ErrorOr<Sample>>;

public class AddSampleCommandHandler(ISampleRepository repository) : IRequestHandler<AddSampleCommand, ErrorOr<Sample>>
{
    public async Task<ErrorOr<Sample>> Handle(AddSampleCommand command, CancellationToken cancellationToken)
    {
        var sample = new Sample(command.Name, command.Description);

        sample.AddEntityId(EntityId.CreateUnique());

        var updatedSample = await repository.AddAsync(sample, cancellationToken);
        return updatedSample;
    }
}
