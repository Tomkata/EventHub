

namespace EventHub.Infrastructure.Configurations
{
    using EventHub.Core.Models;
    using EventHub.Infrastructure.Data.Identity;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    public class EventParticipantConfiguration : IEntityTypeConfiguration<EventParticipant>
    {
        public void Configure(EntityTypeBuilder<EventParticipant> builder)
        {
    
            builder.
                HasKey(x => new { x.EventId,  x.UserId });

            builder
                .HasOne<ApplicationUser>()
                .WithMany()
                .HasForeignKey(x => x.UserId)   
                .OnDelete(DeleteBehavior.Restrict);


            builder
                .HasOne<Event>()    
                .WithMany(x=>x.EventParticipants)
                .HasForeignKey(x => x.EventId)
                .OnDelete(DeleteBehavior.Restrict);

           

        }
    }
}
