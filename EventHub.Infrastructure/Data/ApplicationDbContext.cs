

namespace EventHub.Infrastructure.Data
{
    using EventHub.Core.Models.Common;
    using EventHub.Core.Models.Events;
    using EventHub.Core.Models.Messaging;
    using EventHub.Core.Models.Organizer;
    using EventHub.Core.Models.Social;
    using EventHub.Core.Models.Users;
    using EventHub.Infrastructure.Configurations.Common;
    using EventHub.Infrastructure.Configurations.Event;
    using EventHub.Infrastructure.Configurations.Messaging;
    using EventHub.Infrastructure.Configurations.Organizer;
    using EventHub.Infrastructure.Configurations.Social;
    using EventHub.Infrastructure.Configurations.User;
    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

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
        public virtual DbSet<Location> Locations { get; set; }
        public virtual DbSet<EventParticipant> EventParticipants { get; set; }
        public virtual DbSet<OrganizerRequest> OrganizerRequests { get; set; }
        public virtual DbSet<UserProfile> UserProfiles { get; set; }
        public virtual DbSet<Interest> Interests { get; set; }
        public virtual DbSet<UserProfileInterest> UserProfileInterests { get; set; }
        public virtual DbSet<UserFollow> UserFollows { get; set; }

        public virtual DbSet<Message> Messages { get; set; }

        public virtual DbSet<Conversation> Conversations { get; set; }


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
            builder.ApplyConfiguration(new UserFollowConfiguration());
            builder.ApplyConfiguration(new MessageConfiguration());
            builder.ApplyConfiguration(new ConversationConfiguration());

            base.OnModelCreating(builder);
        }

        protected override void ConfigureConventions(ModelConfigurationBuilder builder)
        {
            builder.Properties<DateTime>()
                .HaveConversion<UtcDateTimeConverter>();
        }


        public class UtcDateTimeConverter : ValueConverter<DateTime, DateTime>
        {
            public UtcDateTimeConverter() : base(
                v => v,
                v => DateTime.SpecifyKind(v, DateTimeKind.Utc))
            { }
        }

    }
}
