namespace EventHub.Infrastructure.Configurations.Social
{
    using EventHub.Core.Models.Social;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    public class UserFollowConfiguration : IEntityTypeConfiguration<UserFollow>
    {
        public void Configure(EntityTypeBuilder<UserFollow> builder)
        {
            builder.HasKey(x => new { x.FollowerId, x.FollowingId });

            builder.HasIndex(x => x.FollowerId);
            builder.HasIndex(x => x.FollowingId);

            builder.HasOne(x => x.Follower)
                .WithMany(p => p.Following)
                .HasForeignKey(x => x.FollowerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Following)
                .WithMany(p => p.Followers)
                .HasForeignKey(x => x.FollowingId)  
                .OnDelete(DeleteBehavior.Restrict);

         
        }
    }
}
