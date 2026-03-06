


namespace EventHub.Infrastructure.Configurations.Messaging
{
    using EventHub.Core.Models.Messaging;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class MessageConfiguration : IEntityTypeConfiguration<Message>
    {
        public void Configure(EntityTypeBuilder<Message> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Content)
             .HasMaxLength(2000)
             .IsRequired();

            builder.HasIndex(x => x.ConversationId);

            builder.HasIndex(x => new { x.ConversationId, x.CreatedAt });

            builder.HasIndex(x => new { x.ConversationId, x.CreatedAt, x.Id });

            builder.HasOne(x => x.Sender)
                .WithMany()
                .HasForeignKey(x => x.SenderId);

            builder.HasOne(x => x.Conversation)
                .WithMany(x => x.Messages)
                .HasForeignKey(x => x.ConversationId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
