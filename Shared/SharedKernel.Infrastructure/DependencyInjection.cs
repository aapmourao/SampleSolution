using System.Text;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

using SharedKernel.Authorization.Interfaces;
using SharedKernel.Infrastructure.Authentication.Decorator;
using SharedKernel.Infrastructure.Authentication.TokenGenerator;
using SharedKernel.Infrastructure.Common.Settings;
using SharedKernel.Infrastructure.EventualConsistency.Options;
using SharedKernel.Infrastructure.IntegrationEvents.BackgroundService;
using SharedKernel.Infrastructure.IntegrationEvents.IntegrationEventsPublisher;
using SharedKernel.Infrastructure.IntegrationEvents.MessageBroker;
using SharedKernel.Infrastructure.IntegrationEvents.Settings;
using SharedKernel.Infrastructure.Persistence;
using SharedKernel.Infrastructure.Persistence.DomainEnums;
using SharedKernel.Infrastructure.Persistence.Provider;
using SharedKernel.Infrastructure.Persistence.Repositories;
using SharedKernel.Infrastructure.Services;
using SharedKernel.Services;

namespace SharedKernel.Infrastructure;

public static class DependencyInjection
{
    private static string _messageBrokerSettingsFileName = "messagebroker.settings.json";

    public static IServiceCollection AddSharedKernelPersistence<T>(
        this IServiceCollection services,
        IConfiguration configuration,
        string connectionStringName) where T : DbContext
    {
        var connectionString = configuration.GetConnectionString(connectionStringName)
            ?? throw new InvalidOperationException($"Connection string '{connectionStringName}' not found.");

        var databaseProviderOptions = new DatabaseSelectorOptions();
        configuration.Bind(DatabaseSelectorOptions.Section, databaseProviderOptions);

        var providerSqlAdapter = ProviderSqlAdapterFactory.CreateProviderSqlAdapter(databaseProviderOptions.Provider);
        services.AddSingleton<IProviderSqlAdapter>(providerSqlAdapter);

        services.AddDbContext<T>(options =>
        {

            if (databaseProviderOptions.Provider is ProviderType.Sqlite)
                options.UseSqlite(connectionString);

            if (databaseProviderOptions.Provider is ProviderType.Postgres)
                options.UseNpgsql(connectionString,
                    options => options.UseAdminDatabase("postgres"));
        });

        services.AddScoped<IOutboxIntegrationEventRepository, OutboxIntegrationEventRepository>();
        services.AddScoped<IInboxIntegrationEventRepository, InboxIntegrationEventRepository>();
        services.AddScoped<IEventualConsistencyMessageRepository, EventualConsistencyMessageRepository>();

        services.AddScoped<IDomainEnumFactory, DomainEnumFactory>();

        return services;
    }

    public static IServiceCollection AddSharedKernelAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        // Register the TenantService with a list of tenants
        services.AddSingleton<ITenantService>(new TenantService(new List<string> { "TenantOne" }));

        // Set Authorize decorator to use JWT
        services.AddScoped<IJwtTokenValidator, JwtTokenValidator>();
        services.AddControllersWithViews(options => options.Filters.Add(typeof(JwtAuthorizeFilter)));

        // Get JwtSettings from appsettings.json
        var jwtSettings = new JwtSettings();
        configuration.Bind(JwtSettings.Section, jwtSettings);
        services.AddScoped<IOptions<JwtSettings>>(provider => Options.Create(jwtSettings));

        // Add Authentication services
        services.AddAuthentication(defaultScheme: JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidAudience = jwtSettings.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwtSettings.Secret)),
                };
                options.Authority = jwtSettings.IdentityUrl;
                options.RequireHttpsMetadata = jwtSettings.RequireHttps;
                options.Audience = jwtSettings.Audience;
            });

        services.AddScoped<IUserIdentityService, UserIdentityService>();
        return services;
    }

    public static IServiceCollection AddSharedKernelMessageBroker(this IServiceCollection services, IConfiguration configuration)
    {
        // Register Notification Handler
        services.AddMediatR(options => options.RegisterServicesFromAssemblyContaining(typeof(DependencyInjection)));

        // Database Selector
        services.AddSingletonOptions<DatabaseSelectorOptions>(configuration, DatabaseSelectorOptions.Section);

        // Hosted Services Selector
        services.Configure<HostedServicesSelectorOptions>(configuration.GetSection(HostedServicesSelectorOptions.Section));

        // Message Broker
        services.AddOptions();
        services.AddCustomSingletonOptions<MessageBrokerSettings>(MessageBrokerSettings.Section, _messageBrokerSettingsFileName);

        services.AddSingletonOptions<MessageBrokerPublisherSettings>(configuration, MessageBrokerPublisherSettings.Section);
        services.AddSingletonOptions<MessageBrokerConsumerSettings>(configuration, MessageBrokerConsumerSettings.Section);

        services.AddSingletonOptions<PublishIntegrationEventsOptions>(configuration, PublishIntegrationEventsOptions.Section);
        services.AddSingletonOptions<EventualConsistencyOptions>(configuration, EventualConsistencyOptions.Section);

        services.AddSingleton<IMessageBrokerFactory, MessageBrokerFactory>();
        services.AddScoped<RabbitMQClient>();

        return services;
    }

    public static void AddSingletonOptions<T>(this IServiceCollection services, IConfiguration configuration, string sectionName) where T : class, new()
    {
        var options = new T();
        configuration.Bind(sectionName, options);
        services.AddSingleton(Options.Create(options));
    }

    public static void AddCustomSingletonOptions<T>(this IServiceCollection services, string sectionName, string configurationFileName) where T : class, new()
    {
        var configuration = new ConfigurationBuilder()
            .AddJsonFile(configurationFileName, optional: false, reloadOnChange: true)
            .AddEnvironmentVariables()
            .Build();

        var options = new T();
        configuration.Bind(sectionName, options);
        services.AddSingleton(Options.Create(options));
    }

    public static IServiceCollection AddSharedKernelBackgroundServices(this IServiceCollection services, IConfiguration configuration)
    {
        var databaseProviderOptions = new HostedServicesSelectorOptions();
        configuration.Bind(HostedServicesSelectorOptions.Section, databaseProviderOptions);

        if (databaseProviderOptions.IntegrationEventsPublisher)
            services.AddSingleton<IIntegrationEventsPublisher, IntegrationEventsPublisher>();

        if (databaseProviderOptions.ConsumeIntegrationEventsBackgroundService)
            services.AddHostedService<ConsumeIntegrationEventsBackgroundService>();

        if (databaseProviderOptions.PublishIntegrationEventsBackgroundService)
            services.AddHostedService<PublishIntegrationEventsBackgroundService>();

        return services;
    }
}