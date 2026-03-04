namespace EventHub.Infrastructure.Configurations.User
{
    using EventHub.Core.Models.Users;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class UserProfileInterestConfiguration : IEntityTypeConfiguration<UserProfileInterest>
    {
        public void Configure(EntityTypeBuilder<UserProfileInterest> builder)
        {
            builder.HasKey(x => new { x.UserId, x.InterestId });

            builder.HasOne(x => x.UserProfile)
                .WithMany(x => x.UserProfileInterests)
                .HasForeignKey(x => x.UserId);

            builder.HasOne(x => x.Interest)
                .WithMany(x => x.UserProfileInterests)
                .HasForeignKey(x => x.InterestId);

        }
    }
}
