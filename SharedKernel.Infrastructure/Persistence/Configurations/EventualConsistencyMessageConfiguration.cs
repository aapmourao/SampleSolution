using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SharedKernel.Infrastructure.EventualConsistency;
internal class EventualConsistencyMessageConfiguration : IEntityTypeConfiguration<EventualConsistencyMessage>
{
    public void Configure(EntityTypeBuilder<EventualConsistencyMessage> builder)
    {
        builder.ToTable("EventualConsistencyMessages");
        builder.HasKey(x => new { x.Id });

        builder.Property(e => e.Id)
            .ValueGeneratedNever();

        builder.Property(e => e.EventName);
        builder.Property(e => e.EventContent);

        builder.Property(e => e.CorrelationId);
        builder.Property(e => e.CorrelationEntity);

        builder.Property(e => e.CreatedAtUtc);

        builder.Property(e => e.ProcessedAtUtc);

        builder.Property(e => e.ProcessedResult);
        builder.Property(e => e.ProcessedMessage);

    }
}