


namespace EventHub
{
    using Microsoft.EntityFrameworkCore;
    using EventHub.Infrastructure.Data;
    using EventHub.Infrastructure.Data.Seed;
    using EventHub.Infrastructure.Identity;
    using EventHub.Services;
    using Microsoft.AspNetCore.Identity;
    using static EventHub.Web.Areas.Identity.IdentityConfigurationSettings.Settings;

    public class Program
    {
        public static async Task Main(string[] args)    
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));

            builder.Services.AddDatabaseDeveloperPageExceptionFilter();

            builder.Services.AddDefaultIdentity<ApplicationUser>()
                .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();


            var identitySection = builder.Configuration.GetSection("IdentitySettings");
            builder.Services.Configure<IdentitySettings>(identitySection);

            var identitySettings = identitySection.Get<IdentitySettings>();

            builder.Services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequiredLength = identitySettings.Password.RequiredLength;
                options.Password.RequireDigit = identitySettings.Password.RequireDigit;
                options.Password.RequireUppercase = identitySettings.Password.RequireUppercase;
                options.Password.RequireLowercase = identitySettings.Password.RequireLowercase;

                options.Lockout.MaxFailedAccessAttempts = identitySettings.Lockout.MaxFailedAttempts;
                options.Lockout.DefaultLockoutTimeSpan =
                    TimeSpan.FromMinutes(identitySettings.Lockout.LockoutMinutes);
            });

            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.SlidingExpiration = identitySettings.Cookie.IsExpiration;

                options.ExpireTimeSpan =
                TimeSpan.FromMinutes(identitySettings.Cookie.ExpireMinutes);
            });

            builder.Services.Configure<SecurityStampValidatorOptions>(opt =>
            {
                opt.ValidationInterval = TimeSpan.FromSeconds(10);
            });

            builder.Services.AddServicesAndRepositories();


            builder.Services.AddControllersWithViews();
            builder.Services.AddRazorPages();

            var app = builder.Build();

            //role seeder
            using (var scope = app.Services.CreateScope())
            {
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                await RoleSeeder.SeedRolesAsync(roleManager);
            }

            using (var scope = app.Services.CreateScope())
            {
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                var adminUser = await userManager.FindByEmailAsync("toma12222@gmail.com");
                if (adminUser!=null && !await userManager.IsInRoleAsync(adminUser,"Admin"))
                {
                    await userManager.AddToRoleAsync(adminUser,"Admin");
                }
            }


            using (var scope = app.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                await DataSeeder.SeedAsync(context);
            }


                // Configure the HTTP request pipeline.
                if (app.Environment.IsDevelopment())
                {
                    app.UseMigrationsEndPoint();
                }
                else
                {
                    app.UseExceptionHandler("/Home/Error");
                    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                    app.UseHsts();
                }

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseStaticFiles();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapStaticAssets();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}")
                .WithStaticAssets();

            app.MapRazorPages()
               .WithStaticAssets();

            app.Run();
        }
    }
}
