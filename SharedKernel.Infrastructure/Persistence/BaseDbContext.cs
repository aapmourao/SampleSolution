using System.Reflection;
using System.Text.Json;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using SharedKernel.Authorization.Interfaces;
using SharedKernel.Domain.Common;
using SharedKernel.Domain.Core;
using SharedKernel.Infrastructure.Common.Model;
using SharedKernel.Infrastructure.Common.Settings;
using SharedKernel.Infrastructure.EventualConsistency;
using SharedKernel.Infrastructure.IntegrationEvents;
using SharedKernel.Infrastructure.Persistence.DomainEnums;
using SharedKernel.Infrastructure.Persistence.Repositories;

namespace SharedKernel.Infrastructure.Persistence;

public class BaseDbContext : DbContext, IDomainEnumDbContext, IInboxDbContext, IOutboxDbContext, IEventDbContext
{
    private readonly IConfiguration _configuration;
    public const string DomainEventsKey = "DomainEvents";
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IPublisher _publisher;
    private readonly IDomainEnumFactory _domainEnumFactory;

    private readonly DatabaseSelectorOptions _databaseSelector;

    private readonly IUserIdentityService _userIdentityService;

    public ProviderType DataBaseProvider => _databaseSelector.Provider;

    public DbSet<EventualConsistencyMessage> EventualConsistencyMessages { get; set; } = null!;
    public DbSet<OutboxIntegrationEvent> OutboxIntegrationEvents { get; set; } = null!;
    public DbSet<InboxIntegrationEvent> InboxIntegrationEvents { get; set; } = null!;
    public DbSet<DomainEnum> DomainEnums { get; set; } = null!;

    public async Task<List<T>> ExecuteSelectQueryAsync<T>(string sqlQuery, CancellationToken cancellationToken) where T : class
    {
        return await this.Set<T>().FromSqlRaw(sqlQuery).ToListAsync(cancellationToken);
    }

    public BaseDbContext(
        DbContextOptions options,
        IHttpContextAccessor httpContextAccessor,
        IPublisher publisher,
        IDomainEnumFactory domainEnumFactory,
        IConfiguration configuration,
        IOptions<DatabaseSelectorOptions> databaseSelectorOptions,
        IUserIdentityService userIdentityService)
        : base(options)
    {
        // set db selector
        _databaseSelector = databaseSelectorOptions.Value;
        _domainEnumFactory = domainEnumFactory;
        _configuration = configuration;
        _userIdentityService = userIdentityService;

        // Create the database if it doesn't exist
        this.Database.EnsureCreated();

        // Ensure that all existing scripts are executed
        EnsureScriptUpdates();

        // #EC-5: Get the http context accessor to hold the queue of domain events
        _httpContextAccessor = httpContextAccessor;
        // Added to provide a way to execute dbContext without a client waiting online for purpose of unit testing
        _publisher = publisher;
    }

    #region SaveChanges
    public async override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // #EC-6: Loop existing domains and extract list of events
        // get hold of all the domain events
        var domainEvents = ChangeTracker.Entries<IAggregateRoot>()
            .Select(entry => entry.Entity.PopDomainEvents())
            .SelectMany(x => x)
            .ToList();

        // perform entities updates
        processChangeTrackerEntries();

        // Save domain events into eventual consistency table
        await ProcessEventualConsistencyAsync(domainEvents, cancellationToken);

        // ToDo: Update Audit fields

        var result = await base.SaveChangesAsync(cancellationToken);

        // store them in the http context for later if user is waiting online
        if (IsUserWaitingOnline())
        {
            AddDomainEventsToOfflineProcessingQueue(domainEvents);
        }
        else
        {
            await PublishDomainEventsAsync(_publisher, domainEvents);
        }

        return result;
    }

    private void processChangeTrackerEntries()
    {
        // #EC-7: Loop through all the entities in the change tracker
        // loop through all the entities in the change tracker
        foreach (var entry in ChangeTracker.Entries())
        {
            var currentUser = _userIdentityService.GetUserName();
            // if the entity is an auditable entity root, then update the audit fields
            if (entry.Entity is IAuditableEntity auditableEntity)
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        auditableEntity.AuditFields.CreatedBy = currentUser;
                        break;
                    case EntityState.Modified:
                        auditableEntity.AuditFields.Update(currentUser);
                        break;
                }
            }

            // if the entity is an auditable entity root, then update the audit fields
            if (entry.Entity is TrackingProcessedStatus trackingProcessedStatus)
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        trackingProcessedStatus.CreatedAtUtc = DateTime.UtcNow;
                        break;
                }
            }
        }
    }

    /// <summary>
    /// Process eventual consistency messages
    /// This method is used to store the domain events in the eventual consistency table
    /// </summary>
    /// <param name="domainEvents"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    private async Task ProcessEventualConsistencyAsync(List<IDomainEvent> domainEvents, CancellationToken cancellationToken)
    {
        var correlationEntity = ChangeTracker.Entries<IAggregateRoot>()
            .Select(entry => $"{entry.Entity.GetType().Name}.{entry.Entity.GetEntityIdValue()}")
            .FirstOrDefault();

        var correlationId = Guid.NewGuid();
        foreach (var item in domainEvents)
        {
            await AddEventualConsistencyMessageAsync(item, correlationId, correlationEntity ?? "No entity", cancellationToken);
        }
    }

    /// <summary>
    /// Add eventual consistency message
    /// This method is used to store the domain event in the eventual consistency table
    /// </summary>
    /// <param name="domainEvent"></param>
    /// <param name="correlationId"></param>
    /// <param name="correlationEntity"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    private async Task AddEventualConsistencyMessageAsync(
        IDomainEvent domainEvent,
        Guid correlationId,
        string correlationEntity,
        CancellationToken cancellationToken)
    {
        Type actualType = domainEvent.GetType();
        var eventualConsistencyMessage = new EventualConsistencyMessage(
            EventName: domainEvent.GetType().Name,
            EventContent: JsonSerializer.Serialize(domainEvent, actualType),
            CorrelationId: correlationId,
            CorrelationEntity: correlationEntity);
        domainEvent.Id = eventualConsistencyMessage.Id;

        await EventualConsistencyMessages.AddAsync(
            eventualConsistencyMessage,
            cancellationToken);
    }

    /// <summary>
    /// Check if the user is waiting online
    /// If the http context is not null, then the user is waiting online
    /// If the http context is null, then the user is not waiting online and the call was made internally (aka: unit tests)
    /// </summary>
    /// <returns></returns>
    private bool IsUserWaitingOnline() => _httpContextAccessor.HttpContext is not null;

    private static async Task PublishDomainEventsAsync(IPublisher publisher, List<IDomainEvent> domainEvents)
    {
        foreach (var domainEvent in domainEvents)
        {
            await publisher.Publish(domainEvent);
        }
    }


    private void AddDomainEventsToOfflineProcessingQueue(List<IDomainEvent> domainEvents)
    {
        // #EC-6: Add domain events to the queue
        // fetch queue from http context or create a new queue if it doesn't exist
        var domainEventsQueue = _httpContextAccessor.HttpContext!.Items
            .TryGetValue(DomainEventsKey, out var value) && value is Queue<IDomainEvent> existingDomainEvents
                ? existingDomainEvents
                : new Queue<IDomainEvent>();

        // add the domain events to the end of the queue
        domainEvents.ForEach(domainEventsQueue.Enqueue);

        // store the queue in the http context
        _httpContextAccessor.HttpContext!.Items[DomainEventsKey] = domainEventsQueue;
    }
    #endregion app

    #region model and scripts

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        modelBuilder.Entity<DbVersion>().HasData(
            new DbVersion { Version = "v0.0.000", Script = "Database created" }
        );

        // Load Instance Default Enums
        var domainEnums = _configuration.GetSection(DomainEnum.ConfigurationName).Get<List<DomainEnum>>() ?? new List<DomainEnum>();
        modelBuilder.Entity<DomainEnum>().HasData(
            domainEnums
        );

        _domainEnumFactory.PopulateEnums(AppDomain.CurrentDomain.GetAssemblies(), domainEnums);
        // base.OnModelCreating(modelBuilder);

    }
    private void EnsureScriptUpdates()
    {
        var connection = this.Database.GetDbConnection();
        var command = connection.CreateCommand();

        // ToDo: Read the scripts from the file system
        command.CommandText = "CREATE TABLE IF NOT EXISTS __Scripts (Id uuid PRIMARY KEY, Description TEXT, Version TEXT, Timestamp TIMESTAMP)";

        connection.Open();
        command.ExecuteNonQuery();
        connection.Close();
    }
    #endregion model and scripts

}