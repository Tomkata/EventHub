
namespace EventHub.Web.Exctensions
{
    using EventHub.Infrastructure.Data;
    using EventHub.Infrastructure.Identity;
    using Microsoft.AspNetCore.Identity;
    using static EventHub.Web.Areas.Identity.IdentityConfigurationSettings.Settings;

    public static class IdentityExtensions
    {
        public static IServiceCollection AddIdentityConfiguration(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddDefaultIdentity<ApplicationUser>()
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>();


            var identitySection = configuration.GetSection("IdentitySettings");
            services.Configure<IdentitySettings>(identitySection);

            var identitySettings = identitySection.Get<IdentitySettings>();

            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequiredLength = identitySettings.Password.RequiredLength;
                options.Password.RequireDigit = identitySettings.Password.RequireDigit;
                options.Password.RequireUppercase = identitySettings.Password.RequireUppercase;
                options.Password.RequireLowercase = identitySettings.Password.RequireLowercase;

                options.Lockout.MaxFailedAccessAttempts = identitySettings.Lockout.MaxFailedAttempts;
                options.Lockout.DefaultLockoutTimeSpan =
                    TimeSpan.FromMinutes(identitySettings.Lockout.LockoutMinutes);
            });

            services.ConfigureApplicationCookie(options =>
            {
                options.SlidingExpiration = identitySettings.Cookie.IsExpiration;

                options.ExpireTimeSpan =
                TimeSpan.FromMinutes(identitySettings.Cookie.ExpireMinutes);
            });

            return services;
        }
    }
}
