namespace CleanTemplate.Api.Controllers;
using System.Threading.Tasks;
using CleanTemplate.Application.Samples.Commands;
using CleanTemplate.Application.Samples.Queries;
using CleanTemplate.Contracts.Sample;
using MediatR;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
public class SampleController : ApiController
{
    private readonly IMediator mediator;

    public SampleController(IMediator mediator)
    {
        this.mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetAsync()
    {
        var result = await mediator.Send(new ListSamplesQuery());
        return result.Match(
            samples => Ok(samples.Select(s => new SampleRecord(s.Id.ToString(), s.Name, s.Description))),  // Map domain entity to DTO
            Problem);
    }

    [HttpGet("{id:Guid}")]
    public async Task<IActionResult> GetAsync(Guid id)
    {
        var result = await mediator.Send(new ListSampleByIdQuery(id));
        return result.Match(
            sample => Ok(sample),  // Map domain entity to DTO
            Problem);
    }

    [HttpPost]
    public async Task<IActionResult> PostAsync([FromBody] SampleRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            return ProblemBadRequest("Name is required");
        }

        if (string.IsNullOrWhiteSpace(request.Description))
        {
            return ProblemBadRequest("Description is required");
        }

        var command = new AddSampleCommand(request.Name, request.Description);
        var result = await mediator.Send(command);
        return result.Match(
            sample => Ok(new SampleRecord(sample.Id.ToString(), sample.Name, sample.Description)),
            Problem);
    }

    [HttpPut]
    public async Task<IActionResult> PutAsync([FromBody] SampleUpdateRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            return ProblemBadRequest("Name is required");
        }

        if (string.IsNullOrWhiteSpace(request.Description))
        {
            return ProblemBadRequest("Description is required");
        }

        var command = new UpdateSampleCommand(request.Id, request.Name, request.Description);
        var result = await mediator.Send(command);
        return result.Match(
            item => Ok(item),
            Problem);
    }
}
