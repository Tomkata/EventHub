

namespace EventHub.Infrastructure.Configurations
{
    using EventHub.Core.Models;
    using EventHub.Infrastructure.Identity;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    public class UserProfileConfiguration : IEntityTypeConfiguration<UserProfile>
    {
        public void Configure(EntityTypeBuilder<UserProfile> builder)
        {
            builder.HasKey(x => x.UserId);

            builder.HasOne<ApplicationUser>()   
                .WithOne()
                .HasForeignKey<UserProfile>(x=>x.UserId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);


        }
    }
}
