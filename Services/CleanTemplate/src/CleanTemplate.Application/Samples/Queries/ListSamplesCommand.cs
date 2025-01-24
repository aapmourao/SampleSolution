using CleanTemplate.Application.Common.Interfaces;
using CleanTemplate.Domain.SampleAggregate;

namespace CleanTemplate.Application.Samples.Queries;

public record ListSamplesQuery : IRequest<ErrorOr<IEnumerable<Sample>>>;

public class ListSamplesQueryHandler : IRequestHandler<ListSamplesQuery, ErrorOr<IEnumerable<Sample>>>
{
    private readonly ISampleRepository repository;

    public ListSamplesQueryHandler(ISampleRepository repository)
    {
        this.repository = repository;
    }

    public async Task<ErrorOr<IEnumerable<Sample>>> Handle(ListSamplesQuery request, CancellationToken cancellationToken)
    {
        var samples = await repository.GetAsync(cancellationToken);
        return samples ?? default!;
    }
}
