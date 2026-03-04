namespace EventHub.Infrastructure.Configurations.Organizer
{
    using EventHub.Core.Models.Organizer;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class OrganizerRequestConfiguration : IEntityTypeConfiguration<OrganizerRequest>
    {
        public void Configure(EntityTypeBuilder<OrganizerRequest> builder)
        {
            builder.
                  HasIndex(x => x.UserId)
                  .IsUnique();

        }
    }
}
