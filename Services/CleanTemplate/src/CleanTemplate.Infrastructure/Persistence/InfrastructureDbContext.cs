using System.Reflection;
using CleanTemplate.Domain.SampleAggregate;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using SharedKernel.Infrastructure.Common.Settings;
using SharedKernel.Infrastructure.Persistence;
using SharedKernel.Infrastructure.Persistence.DomainEnums;

namespace CleanTemplate.Infrastructure.Persistence;

public class InfrastructureDbContext(
        DbContextOptions options,
        IHttpContextAccessor httpContextAccessor,
        IPublisher publisher,
        IDomainEnumFactory domainEnumFactory,
        IConfiguration configuration,
        IOptions<DatabaseSelectorOptions> databaseSelectorOptions)
        : BaseDbContext(options, httpContextAccessor, publisher, domainEnumFactory, configuration, databaseSelectorOptions)
{
    public DbSet<Sample> Samples { get; set; } = null!;
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(modelBuilder);

    }
}
