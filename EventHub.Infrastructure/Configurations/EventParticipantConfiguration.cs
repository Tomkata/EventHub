using EventHub.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace EventHub.Infrastructure.Configurations
{
    public class EventParticipantConfiguration : IEntityTypeConfiguration<EventParticipant>
    {
        public void Configure(EntityTypeBuilder<EventParticipant> builder)
        {
            builder.
                HasKey(x => new { x.EventId, x.UserId });

            builder
                .HasOne<ApplicationUser>()
                .WithMany()
                .HasForeignKey(x => x.UserId);

            builder
                .HasOne<Event>()
                .WithMany()
                .HasForeignKey(x => x.EventId);

        }
    }
}
