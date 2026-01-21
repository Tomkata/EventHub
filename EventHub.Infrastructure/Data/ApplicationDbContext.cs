

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

            builder.Entity<Category>().HasData(
       new Category { Id = Guid.Parse("11111111-1111-1111-1111-111111111111"), Name = "Concert" },
       new Category { Id = Guid.Parse("22222222-2222-2222-2222-222222222222"), Name = "Conference" },
       new Category { Id = Guid.Parse("33333333-3333-3333-3333-333333333333"), Name = "Sports" },
       new Category { Id = Guid.Parse("44444444-4444-4444-4444-444444444444"), Name = "Exhibition" },
       new Category { Id = Guid.Parse("55555555-5555-5555-5555-555555555555"), Name = "Workshop" },
       new Category { Id = Guid.Parse("66666666-6666-6666-6666-666666666666"), Name = "Festival" },
       new Category { Id = Guid.Parse("77777777-7777-7777-7777-777777777777"), Name = "Meetup" }
   );




            base.OnModelCreating(builder);
        }
    }
}
