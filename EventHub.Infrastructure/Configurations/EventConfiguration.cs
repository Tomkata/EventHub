using EventHub.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace EventHub.Infrastructure.Configurations
{
    public class EventConfiguration : IEntityTypeConfiguration<Event>
    {
        public void Configure(EntityTypeBuilder<Event> builder)
        {
            builder.HasKey(x => x.Id);

            builder
                 .HasOne(x => x.Location)
                 .WithMany(x => x.Events)
                 .HasForeignKey(x => x.LocationId)
                 .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasOne(x => x.Category)
                .WithMany(x => x.Events)
                .HasForeignKey(x => x.CategoryId)
                 .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasOne<ApplicationUser>()
                .WithMany()
                .HasForeignKey(x=>x.OrganizerId)
                 .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
