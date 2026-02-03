using EventHub.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Identity.Client;

namespace EventHub.Infrastructure.Configurations
{
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
