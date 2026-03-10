namespace EventHub.Infrastructure.Configurations.Event
{
    using EventHub.Core.Models.Events;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    public class EventParticipantConfiguration : IEntityTypeConfiguration<EventParticipant>
    {
        public void Configure(EntityTypeBuilder<EventParticipant> builder)
        {

            builder.
                HasKey(x => new { x.EventId, x.UserId });   

            builder.HasIndex(x => x.UserId);

            builder
                .HasOne(x=>x.Event)    
                .WithMany(x=>x.EventParticipants)
                .HasForeignKey(x => x.EventId)
                .OnDelete(DeleteBehavior.Cascade);

            builder
              .HasOne(ep => ep.UserProfile)
              .WithMany()
              .HasForeignKey(ep => ep.UserId)
              .OnDelete(DeleteBehavior.Restrict);




        }
    }
}
