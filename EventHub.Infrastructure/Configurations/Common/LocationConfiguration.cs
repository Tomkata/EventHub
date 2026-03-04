namespace EventHub.Infrastructure.Configurations.Common
{
    using EventHub.Core.Models.Common;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    public class LocationConfiguration : IEntityTypeConfiguration<Location>
    {
        public void Configure(EntityTypeBuilder<Location> builder)
        {
            builder.HasKey(x => x.Id);


                builder
                .Property(x => x.City)
                .IsRequired()
                .HasMaxLength(100);

                builder
                .HasIndex(l => new { l.City, l.Zip })
                .IsUnique();

                builder
                .Property(x => x.Zip)
                .IsRequired();

        }
    }
}
