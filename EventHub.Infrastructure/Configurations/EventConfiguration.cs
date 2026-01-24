

namespace EventHub.Infrastructure.Configurations
{
    using EventHub.Core.Models;
    using EventHub.Infrastructure.Data.Identity;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
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
                .Property(x => x.Address)
                .IsRequired()
                .HasMaxLength(100);

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


            List<Event> events = new List<Event>
            {
               new Event
            {
                Id = Guid.Parse("A0000001-0000-0000-0000-000000000001"),
                Title = "Rock the Night Festival",
                Description = "Open-air rock concert featuring Bulgarian bands.",
                ImagePath = "images/events/concert.jpg",
                CreatedAt = new DateTime(2025, 1, 1),
                StartDate = new DateTime(2026, 6, 20, 18, 0, 0),
                EndDate = new DateTime(2026, 6, 20, 23, 30, 0),
                MaxParticipants = 800,
                CategoryId = Guid.Parse("11111111-1111-1111-1111-111111111111"), // Concert
                LocationId = Guid.Parse("46EE85A7-5DA3-42B3-96D2-B58FD2B8CFC9"), // Plovdiv
                Address = "Rowing Canal, Plovdiv",
                OrganizerId = "9dbe8efe-5e69-44a9-aacb-e75f81724fc5"
            },
            new Event
            {
                Id = Guid.Parse("A0000002-0000-0000-0000-000000000002"),
                Title = "Tech Innovators Conference 2026",
                Description = "Annual conference for software engineers and tech leaders.",
                ImagePath = "images/events/conference.jpg",
                CreatedAt = new DateTime(2025, 1, 5),
                StartDate = new DateTime(2026, 3, 15, 9, 0, 0),
                EndDate = new DateTime(2026, 3, 16, 17, 0, 0),
                MaxParticipants = 600,
                CategoryId = Guid.Parse("22222222-2222-2222-2222-222222222222"), // Conference
                LocationId = Guid.Parse("B285237B-5DDC-449F-BF3A-C9CF5E805910"), // Sofia
                Address = "Inter Expo Center, Sofia",
                OrganizerId = "9dbe8efe-5e69-44a9-aacb-e75f81724fc5"
            },
            new Event
            {
                Id = Guid.Parse("A0000003-0000-0000-0000-000000000003"),
                Title = "City Half Marathon",
                Description = "Annual half marathon open to professionals and amateurs.",
                ImagePath = "images/events/spot-event.jpg",
                CreatedAt = new DateTime(2025, 1, 10),
                StartDate = new DateTime(2026, 4, 10, 8, 0, 0),
                EndDate = new DateTime(2026, 4, 10, 14, 0, 0),
                MaxParticipants = 1000,
                CategoryId = Guid.Parse("33333333-3333-3333-3333-333333333333"), // Sports
                LocationId = Guid.Parse("2975D8E3-7C0C-4A92-A7E2-655B2CB349F4"), // Varna
                Address = "Sea Garden, Varna",
                OrganizerId = "9dbe8efe-5e69-44a9-aacb-e75f81724fc5"
            },
            new Event
            {
                Id = Guid.Parse("A0000004-0000-0000-0000-000000000004"),
                Title = "Modern Art Expo",
                Description = "Exhibition of contemporary Bulgarian artists.",
                ImagePath = "images/events/spot-event.jpg",
                CreatedAt = new DateTime(2025, 1, 12),
                StartDate = new DateTime(2026, 5, 5, 10, 0, 0),
                EndDate = new DateTime(2026, 5, 20, 19, 0, 0),
                MaxParticipants = 300,
                CategoryId = Guid.Parse("44444444-4444-4444-4444-444444444444"), //     
                LocationId = Guid.Parse("C85B0784-EA5B-4E53-B0E1-527E5963BFEF"), // Veliko Tarnovo
                Address = "Art Gallery Boris Denev",
                OrganizerId = "9dbe8efe-5e69-44a9-aacb-e75f81724fc5"
            },
            new Event
            {
                Id = Guid.Parse("A0000005-0000-0000-0000-000000000005"),
                Title = "ASP.NET Core Hands-on Workshop",
                Description = "Practical workshop covering EF Core and Web APIs.",
                ImagePath = "images/events/workshop.jpg",
                CreatedAt = new DateTime(2025, 1, 15),
                StartDate = new DateTime(2026, 2, 22, 10, 0, 0),
                EndDate = new DateTime(2026, 2, 22, 17, 0, 0),
                MaxParticipants = 50,
                CategoryId = Guid.Parse("55555555-5555-5555-5555-555555555555"), // Workshop
                LocationId = Guid.Parse("D80720A5-6E69-44A5-87DD-997DF1E4DDC8"), // Ruse
                Address = "Tech Hub Ruse",
                OrganizerId = "9dbe8efe-5e69-44a9-aacb-e75f81724fc5"
            },
            new Event
            {
                Id = Guid.Parse("A0000006-0000-0000-0000-000000000006"),
                Title = "Summer Food & Music Festival",
                Description = "Street food, live music, and local craft beer.",
                ImagePath = "images/events/festival-event.jpg",
                CreatedAt = new DateTime(2025, 1, 18),
                StartDate = new DateTime(2026, 7, 10, 12, 0, 0),
                EndDate = new DateTime(2026, 7, 12, 23, 0, 0),
                MaxParticipants = 900,
                CategoryId = Guid.Parse("66666666-6666-6666-6666-666666666666"), // Festival
                LocationId = Guid.Parse("FA600614-8AA2-441F-86FA-3B04BD8FB796"), // Burgas
                Address = "Central Beach, Burgas",
                OrganizerId = "9dbe8efe-5e69-44a9-aacb-e75f81724fc5"
            },
            new Event
            {
                Id = Guid.Parse("A0000007-0000-0000-0000-000000000007"),
                Title = "Startup Founders Meetup",
                Description = "Networking meetup for startup founders and investors.",
                ImagePath = "images/events/meet-up.jpg",
                CreatedAt = new DateTime(2025, 1, 20),
                StartDate = new DateTime(2026, 1, 30, 18, 30, 0),
                EndDate = new DateTime(2026, 1, 30, 21, 0, 0),
                MaxParticipants = 120,
                CategoryId = Guid.Parse("77777777-7777-7777-7777-777777777777"), // Meetup
                LocationId = Guid.Parse("B285237B-5DDC-449F-BF3A-C9CF5E805910"), // Sofia
                Address = "Coworking Space Sofia",
                OrganizerId = "9dbe8efe-5e69-44a9-aacb-e75f81724fc5"
            }
            };

            builder.HasData(events);

        }
    }
}
