using CleanTemplate.Application.Common.Interfaces;
using CleanTemplate.Domain.SampleAggregate;

namespace CleanTemplate.Application.Samples.Queries;

public record ListSampleByIdQuery(Guid Id) : IRequest<ErrorOr<Sample>>;

public class ListSampleByIdQueryHandler : IRequestHandler<ListSampleByIdQuery, ErrorOr<Sample>>
{
    private readonly ISampleRepository repository;

    public ListSampleByIdQueryHandler(ISampleRepository repository)
    {
        this.repository = repository;
    }

    public async Task<ErrorOr<Sample>> Handle(ListSampleByIdQuery request, CancellationToken cancellationToken)
    {
        var sample = await repository.GetByIdAsync(request.Id, cancellationToken);
        return sample ?? default!;
    }
}
