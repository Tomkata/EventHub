


namespace EventHub
{
    using EventHub.Services;
    using EventHub.Services.Mapping;
    using EventHub.Web.Filter;
    using EventHub.Web.Filters;
    using EventHub.Web.Hubs;
    using Microsoft.AspNetCore.Identity;
    using EventHub.Infrastructure.Extensions;
    using EventHub.Web.Exctensions;
    using static EventHub.Web.Areas.Identity.IdentityConfigurationSettings.Settings;

    public class Program
    {
        public static async Task Main(string[] args)
        {
            try
            {
                var builder = WebApplication.CreateBuilder(args);

                //Service Registration

                builder.Services
                .AddDatabase(builder.Configuration)
                .AddIdentityConfiguration(builder.Configuration)
                .AddServicesAndRepositories()
                .AddRateLimiting()
                .AddDatabaseDeveloperPageExceptionFilter();



                builder.Services.Configure<SecurityStampValidatorOptions>(opt =>
                {
                    opt.ValidationInterval = TimeSpan.FromMinutes(30);
                });


                builder.Services.AddAutoMapper(typeof(Program).Assembly,
                    typeof(ServiceMappingProfile).Assembly);

                builder.Services.AddControllersWithViews(options =>
                {
                    options.Filters.Add<DomainExceptionFilter>();
                    options.Filters.Add<PerformanceMonitoringFilter>();
                });


                builder.Services.AddSignalR(options =>
                {
                    options.EnableDetailedErrors = builder.Environment.IsDevelopment();
                });

                builder.Services.AddRazorPages();



                //Build app

                var app = builder.Build();



                //Database seeder
                try
                {
                    await SeedExtensions.SeedDatabaseAsync(app.Services);
                }
                catch (Exception ex)
                {
                    var logger = app.Services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "Database seeding failed: {Message}", ex.Message);
                }



                // Configure the HTTP request pipeline
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

                app.UseHttpsRedirection();
                app.UseStaticFiles();
                app.UseStatusCodePagesWithReExecute("/Home/StatusCode/{0}");

                app.UseRouting();

                app.UseAuthentication();
                app.UseAuthorization();

                app.MapStaticAssets();

                app.UseRateLimiter();

                app.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}")
                    .WithStaticAssets();

                app.MapRazorPages()
                   .WithStaticAssets();

                app.MapHub<ChatHub>("/chatHub");


                var port = Environment.GetEnvironmentVariable("PORT") ?? "10000";
                app.Run($"http://0.0.0.0:{port}");

            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"STARTUP CRASH: {ex}");
                throw;
            }
        }
          
    }
}
