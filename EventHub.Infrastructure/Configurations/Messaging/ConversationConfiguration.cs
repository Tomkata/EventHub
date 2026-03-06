

namespace EventHub.Infrastructure.Configurations.Messaging
{

    using EventHub.Core.Models.Messaging;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    public class ConversationConfiguration : IEntityTypeConfiguration<Conversation>
    {
        public void Configure(EntityTypeBuilder<Conversation> builder)
        {
            builder.HasKey(x => x.Id);

            builder.HasIndex(x => new {x.User1Id,x.User2Id })
                .IsUnique();

            builder.HasIndex(x => new { x.Id, x.CreatedAt });


            builder.HasOne(x => x.User1)
                .WithMany()
                .HasForeignKey(x => x.User1Id);

            builder.HasOne(x => x.User2)
                .WithMany()
                .HasForeignKey(x => x.User2Id);



        }
    }
}
