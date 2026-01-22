

namespace EventHub.Infrastructure.Configurations
{
    using EventHub.Core.Models;
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
            .Property(x => x.Country)
            .IsRequired()
            .HasMaxLength(100);

            builder
            .Property(x => x.Zip)
            .IsRequired();

        }
    }
}
