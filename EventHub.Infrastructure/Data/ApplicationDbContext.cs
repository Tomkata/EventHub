

namespace EventHub.Infrastructure.Data
{
    using EventHub.Core.Models;
    using EventHub.Infrastructure.Configurations;
    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore;
    using System.Reflection.Emit;
    using System.Runtime.CompilerServices;

    public class ApplicationDbContext
        : IdentityDbContext
    {
        public ApplicationDbContext(
            DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Event> Events { get; set; }
        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<Location>  Locations { get; set; }
        public virtual DbSet<EventParticipant> EventParticipants { get; set; }



        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfiguration(new LocationConfiguration());
            builder.ApplyConfiguration(new CategoryConfiguration());
            builder.ApplyConfiguration(new EventConfiguration());
            builder.ApplyConfiguration(new EventParticipantConfiguration());
          
            base.OnModelCreating(builder);
        }
    }
}
