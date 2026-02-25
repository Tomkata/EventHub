

namespace EventHub.Infrastructure.Data
{
    using EventHub.Core.Models;
    using EventHub.Infrastructure.Configurations;
    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore;

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
        public virtual DbSet<OrganizerRequest> OrganizerRequests { get; set; }
        public virtual DbSet<UserProfile> UserProfiles { get; set; }
        public virtual DbSet<Interest>  Interests { get; set; }
        public virtual DbSet<UserProfileInterest>  UserProfileInterests { get; set; }


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
            builder.ApplyConfiguration(new OrganizerRequestConfiguration());
            builder.ApplyConfiguration(new UserProfileInterestConfiguration());
            builder.ApplyConfiguration(new UserProfileConfiguration());


            base.OnModelCreating(builder);
        }
    }
}
