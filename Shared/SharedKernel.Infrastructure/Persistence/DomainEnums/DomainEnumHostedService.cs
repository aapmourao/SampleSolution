using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SharedKernel.Infrastructure.Persistence.Repositories;

namespace SharedKernel.Infrastructure.Persistence.DomainEnums;
public class DomainEnumHostedService : IHostedService
{
    private readonly IDomainEnumFactory _domainEnumFactory;
    private readonly IServiceProvider _serviceProvider;

    public DomainEnumHostedService(
        IServiceProvider serviceProvider,
        IDomainEnumFactory domainEnumFactory
        )
    {
        _serviceProvider = serviceProvider;
        _domainEnumFactory = domainEnumFactory;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<IDomainEnumDbContext>();
            var categories = await dbContext.DomainEnums.ToListAsync(cancellationToken);
            _domainEnumFactory.PopulateEnums(AppDomain.CurrentDomain.GetAssemblies(), categories);
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}