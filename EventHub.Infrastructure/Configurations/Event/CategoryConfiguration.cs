namespace EventHub.Infrastructure.Configurations.Event
{
    using EventHub.Core.Models.Events;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    public class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.HasKey(x => x.Id);

            builder
                .Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.HasData(
      new Category { Id = Guid.Parse("11111111-1111-1111-1111-111111111111"), Name = "Concert" },
      new Category { Id = Guid.Parse("22222222-2222-2222-2222-222222222222"), Name = "Conference" },
      new Category { Id = Guid.Parse("33333333-3333-3333-3333-333333333333"), Name = "Sports" },
      new Category { Id = Guid.Parse("44444444-4444-4444-4444-444444444444"), Name = "Exhibition" },
      new Category { Id = Guid.Parse("55555555-5555-5555-5555-555555555555"), Name = "Workshop" },
      new Category { Id = Guid.Parse("66666666-6666-6666-6666-666666666666"), Name = "Festival" },
      new Category { Id = Guid.Parse("77777777-7777-7777-7777-777777777777"), Name = "Meetup" });
        }
    }
}
