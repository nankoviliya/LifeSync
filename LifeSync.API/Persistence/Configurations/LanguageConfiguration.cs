using LifeSync.API.Models.Languages;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LifeSync.API.Persistence.Configurations
{
    internal sealed class LanguageConfiguration : IEntityTypeConfiguration<Language>
    {
        public void Configure(EntityTypeBuilder<Language> builder)
        {
            builder.ToTable("Languages");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name)
                .HasMaxLength(100);

            builder.Property(x => x.Code)
            .HasMaxLength(20);
        }
    }
}
