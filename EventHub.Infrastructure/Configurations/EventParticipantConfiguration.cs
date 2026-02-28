

namespace EventHub.Infrastructure.Configurations
{
    using EventHub.Core.Models;
    using EventHub.Infrastructure.Identity;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    public class EventParticipantConfiguration : IEntityTypeConfiguration<EventParticipant>
    {
        public void Configure(EntityTypeBuilder<EventParticipant> builder)
        {

            builder.
                HasKey(x => new { x.EventId, x.UserId });

            //builder.HasIndex(x => new { x.EventId, x.UserId })
            //    .IsUnique();

            builder
                .HasOne(x=>x.Event)    
                .WithMany(x=>x.EventParticipants)
                .HasForeignKey(x => x.EventId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
              .HasOne(ep => ep.UserProfile)
              .WithMany()
              .HasForeignKey(ep => ep.UserId)
              .OnDelete(DeleteBehavior.Restrict);




        }
    }
}
