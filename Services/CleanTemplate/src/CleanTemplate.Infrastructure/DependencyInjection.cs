using CleanTemplate.Application.Common.Interfaces;
using CleanTemplate.Infrastructure.Persistence;
using CleanTemplate.Infrastructure.Persistence.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SharedKernel.Infrastructure;
using SharedKernel.Infrastructure.Persistence.Repositories;

namespace CleanTemplate.Infrastructure;

public static class DependencyInjection
{
    private static string connectionStringName = "CleanTemplate";

    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddAuthentication(configuration)
            .AddMediatR()
            .AddConfigurations(configuration)
            .AddSharedKernelBackgroundServices(configuration)
            .AddPersistence(configuration);

        return services;
    }

    public static IServiceCollection AddMediatR(this IServiceCollection services)
    {
        services.AddMediatR(options => options.RegisterServicesFromAssemblyContaining(typeof(DependencyInjection)));

        return services;
    }

    public static IServiceCollection AddConfigurations(this IServiceCollection services, IConfiguration configuration)
    {
        // services.AddSharedKernelMessageBroker(configuration);
        return services;
    }

    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSharedKernelPersistence<InfrastructureDbContext>(configuration, connectionStringName);
        services.AddScoped<IInboxDbContext, InfrastructureDbContext>();
        services.AddScoped<IOutboxDbContext, InfrastructureDbContext>();
        services.AddScoped<IEventDbContext, InfrastructureDbContext>();
        services.AddScoped<IDomainEnumDbContext, InfrastructureDbContext>();

        services.AddScoped<ISampleRepository, SampleRepository>();

        return services;
    }

    public static IServiceCollection AddAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSharedKernelAuthentication(configuration);
        return services;
    }
}
