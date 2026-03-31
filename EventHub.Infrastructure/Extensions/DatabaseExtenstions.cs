

using EventHub.Infrastructure.Data;
using EventHub.Infrastructure.Interceptors;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EventHub.Infrastructure.Extensions
{
    public static class DatabaseExtenstions
    {
        public static IServiceCollection AddDatabase(
            this IServiceCollection services,
            IConfiguration configuration
            )
        {

            var connectionString = configuration.GetConnectionString("DefaultConnection");

            if (string.IsNullOrEmpty(connectionString))
            {
                connectionString = "Server=localhost;Database=EventHub;Trusted_Connection=True;";
            }

            services.AddSingleton<SlowQueryInterceptor>();

            services.AddDbContext<ApplicationDbContext>((serviceProvider, options) =>
            {
                options.UseSqlServer(connectionString);

                var interceptor = serviceProvider.GetRequiredService<SlowQueryInterceptor>();
                options.AddInterceptors(interceptor);
            });

            return services;
        }
    }
}   
