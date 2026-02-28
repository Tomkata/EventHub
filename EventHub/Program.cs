


namespace EventHub
{
    using EventHub.Infrastructure.Data;
    using EventHub.Infrastructure.Data.Seed;
    using EventHub.Infrastructure.Identity;
    using EventHub.Services;
    using EventHub.Services.Mapping;
    using EventHub.Web.Filter;
    using EventHub.Web.Filters;
    using EventHub.Web.Middleware;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging.Console;
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

            builder.Services.AddAutoMapper(typeof(Program).Assembly,
                typeof(ServiceMappingProfile).Assembly);

            builder.Services.AddControllersWithViews(options =>
            {
                options.Filters.Add<DomainExceptionFilter>();
                options.Filters.Add<PerformanceMonitoringFilter>();
            });


            builder.Services.AddRazorPages();

            var app = builder.Build();

            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;

                var context = services.GetRequiredService<ApplicationDbContext>();
                await context.Database.MigrateAsync();

                var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
                var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

                await IdentitySeeder.SeedAsync(userManager, roleManager);
                await DataSeeder.SeedAsync(context);
                await EventSeeder.SeedAsync(context, userManager);
                await InterestSeeder.SeedAsync(context);
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
