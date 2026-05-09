using LifeSync.API.Models.Tags;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LifeSync.API.Persistence.Configurations;

internal sealed class TagAssignmentConfiguration : IEntityTypeConfiguration<TagAssignment>
{
    public void Configure(EntityTypeBuilder<TagAssignment> builder)
    {
        builder.ToTable("TagAssignments");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.TagId).IsRequired();
        builder.Property(x => x.UserId).IsRequired();
        builder.Property(x => x.EntityId).IsRequired();
        builder.Property(x => x.EntityType).IsRequired().HasConversion<int>();

        // Hard-deleted on unassign — no query filter.

        builder.HasIndex(x => new { x.TagId, x.EntityId, x.EntityType }).IsUnique();
        builder.HasIndex(x => new { x.EntityId, x.EntityType });
    }
}
