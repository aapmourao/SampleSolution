using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SharedKernel.Infrastructure.IntegrationEvents;

namespace SharedKernel.Infrastructure.Persistence.Configurations;

public class OutboxIntegrationEventConfigurations : IEntityTypeConfiguration<OutboxIntegrationEvent>
{
    public void Configure(EntityTypeBuilder<OutboxIntegrationEvent> builder)
    {
        builder.ToTable("OutboxIntegrationEvents");
        builder.HasKey(x => new { x.Id });

        builder.Property(e => e.Id)
            .ValueGeneratedOnAdd();

        builder.Property(o => o.EventName);
        builder.Property(o => o.EventContent);
        builder.Property(e => e.CorrelationId);

        builder.Property(o => o.CreatedAtUtc);

        builder.Property(e => e.ProcessedAtUtc);
        builder.HasIndex(e => e.ProcessedAtUtc);

        builder.Property(e => e.ProcessedResult);
        builder.Property(e => e.ProcessedMessage);

    }
}
