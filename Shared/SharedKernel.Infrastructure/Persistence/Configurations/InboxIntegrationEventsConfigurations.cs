using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SharedKernel.Infrastructure.IntegrationEvents;

namespace SharedKernel.Infrastructure.Persistence.Configurations;

public class InboxIntegrationEventConfigurations : IEntityTypeConfiguration<InboxIntegrationEvent>
{
    public void Configure(EntityTypeBuilder<InboxIntegrationEvent> builder)
    {
        builder.ToTable("InboxIntegrationEvents");
        builder.HasKey(x => new { x.Id });

        builder.Property(e => e.Id)
            .ValueGeneratedOnAdd();

        builder.Property(o => o.Publisher);
        builder.Property(o => o.MessageType);
        builder.Property(e => e.CorrelationId);
        builder.Property(e => e.TimeStamp);

        builder.Property(o => o.CreatedAtUtc);

        builder.Property(e => e.ProcessedAtUtc);

        builder.Property(e => e.ProcessedResult);
        builder.Property(e => e.ProcessedMessage);

    }
}
