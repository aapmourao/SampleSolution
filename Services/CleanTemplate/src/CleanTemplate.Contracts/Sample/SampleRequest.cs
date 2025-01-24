namespace CleanTemplate.Contracts.Sample;

public record SampleRequest(string Name, string Description);
public record SampleUpdateRequest(Guid Id, string? Name, string? Description);
