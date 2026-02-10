
namespace EventHub.Infrastructure.Configurations
{
    using EventHub.Core.Models;
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
