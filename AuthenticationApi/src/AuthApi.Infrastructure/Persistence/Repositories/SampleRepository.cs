using AuthApi.Application.Common.Interfaces;
using AuthApi.Domain.SampleAggregate;
using Microsoft.EntityFrameworkCore;
using static AuthApi.Domain.SampleAggregate.Sample;

namespace AuthApi.Infrastructure.Persistence.Repositories;

public class SampleRepository(InfrastructureDbContext dbContext) : ISampleRepository
{
    public async Task<Sample> AddAsync(Sample sample, CancellationToken cancellationToken)
    {
        var result = await dbContext.AddAsync(sample, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        return result.Entity;
    }

    public async Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken)
    {
        return await dbContext.Samples.AnyAsync(sample => sample.Name == name, cancellationToken);
    }

    public async Task<Sample?> GetByNameAsync(string name, CancellationToken cancellationToken)
    {
        return await dbContext.Samples.FirstOrDefaultAsync(sample => sample.Name == name, cancellationToken);
    }

    public async Task<Sample?> GetByIdAsync(Guid sampleId, CancellationToken cancellationToken)
    {
        var id = new SampleId(sampleId);
        return await dbContext.Samples.FirstOrDefaultAsync(sample => sample.Id == id, cancellationToken);
    }

    public async Task<Sample> UpdateAsync(Sample sample, CancellationToken cancellationToken)
    {
        var updatedSample = dbContext.Update(sample);
        await dbContext.SaveChangesAsync(cancellationToken);
        return updatedSample.Entity;
    }

    public async Task DeleteAsync(Sample sample, CancellationToken cancellationToken)
    {
        dbContext.Remove(sample);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public Task<List<Sample>> GetAsync(CancellationToken cancellationToken)
    {
        return dbContext.Samples.ToListAsync(cancellationToken);
    }
}
