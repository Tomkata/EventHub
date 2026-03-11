


namespace EventHub
{
    using EventHub.Core.Models.Users;
    using EventHub.Infrastructure.Data;
    using EventHub.Infrastructure.Data.Seed;
    using EventHub.Infrastructure.Identity;
    using EventHub.Infrastructure.Interceptors;
    using EventHub.Services;
    using EventHub.Services.Mapping;
    using EventHub.Web.Filter;
    using EventHub.Web.Filters;
    using EventHub.Web.Hubs;
    using EventHub.Web.Middleware;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.CodeAnalysis.Elfie.Diagnostics;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging.Console;
    using static EventHub.Web.Areas.Identity.IdentityConfigurationSettings.Settings;

    public class Program
    {
        public static async Task Main(string[] args)
        {

            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddSingleton<SlowQueryInterceptor>();

            // Add services to the container.
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

            builder.Services.AddDbContext<ApplicationDbContext>((serviceProvider, options) =>
            {
                options.UseSqlServer(connectionString);

                var interceptor = serviceProvider.GetRequiredService<SlowQueryInterceptor>();
                options.AddInterceptors(interceptor);
            });

            builder.Services.AddDatabaseDeveloperPageExceptionFilter();


            builder.Services.AddDefaultIdentity<ApplicationUser>()
                .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();


            builder.Services.AddSignalR(options =>
            {
                options.EnableDetailedErrors = true; 
            });

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

            builder.Services.AddAutoMapper(typeof(Program).Assembly,
                typeof(ServiceMappingProfile).Assembly);

            builder.Services.AddControllersWithViews(options =>
            {
                options.Filters.Add<DomainExceptionFilter>();
                options.Filters.Add<PerformanceMonitoringFilter>();
            });


            builder.Services.AddRazorPages();

            var app = builder.Build();


            var logger = app.Services.GetRequiredService<ILogger<Program>>();

            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var context = services.GetRequiredService<ApplicationDbContext>();
                var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
                var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

                try
                {
                    await context.Database.MigrateAsync();
                    logger.LogInformation("[SEED] Миграции: OK");

                    await IdentitySeeder.SeedAsync(userManager, roleManager);
                    logger.LogInformation("[SEED] IdentitySeeder: OK");

                    context.ChangeTracker.Clear();
                    logger.LogInformation("[SEED] ChangeTracker изчистен");

                    await DataSeeder.SeedAsync(context);
                    logger.LogInformation("[SEED] DataSeeder: OK");

                    var adminUser = await userManager.FindByEmailAsync("admin@eventhub.com");
                    var orgUser = await userManager.FindByEmailAsync("organizer@eventhub.com");

                    logger.LogInformation("[SEED] admin: {Id}", adminUser?.Id ?? "NULL");
                    logger.LogInformation("[SEED] organizer: {Id}", orgUser?.Id ?? "NULL");

                    if (adminUser != null && !await context.UserProfiles.AnyAsync(p => p.UserId == adminUser.Id))
                    {
                        context.UserProfiles.Add(new UserProfile
                        {
                            UserId = adminUser.Id,
                            FirstName = "Admin",
                            LastName = "User",
                            CreatedAt = DateTime.UtcNow
                        });
                        var rows = await context.SaveChangesAsync();
                        logger.LogInformation("[SEED] Admin профил записан: {Rows} реда", rows);
                    }

                    if (orgUser != null && !await context.UserProfiles.AnyAsync(p => p.UserId == orgUser.Id))
                    {
                        context.UserProfiles.Add(new UserProfile
                        {
                            UserId = orgUser.Id,
                            FirstName = "Event",
                            LastName = "Organizer",
                            CreatedAt = DateTime.UtcNow
                        });
                        var rows = await context.SaveChangesAsync();
                        logger.LogInformation("[SEED] Organizer профил записан: {Rows} реда", rows);
                    }

                    await EventSeeder.SeedAsync(context, userManager);
                    logger.LogInformation("[SEED] EventSeeder: OK");

                    await InterestSeeder.SeedAsync(context);
                    logger.LogInformation("[SEED] InterestSeeder: OK");
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "[SEED] ГРЕШКА: {Message}", ex.Message);
                    throw;
                }
            }


            // Configure the HTTP request pipeline..
            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }
            Console.WriteLine(app.Environment.EnvironmentName);

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapStaticAssets();


            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}")
                .WithStaticAssets();

            app.MapRazorPages()
               .WithStaticAssets();

            app.MapHub<ChatHub>("/chatHub");

            app.Run();
        }
    }
}
