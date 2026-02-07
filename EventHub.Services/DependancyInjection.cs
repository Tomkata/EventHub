using EventHub.Repositories.Repositories;
using EventHub.Services.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EventHub.Services
{
    public static class DependencyInjection
    {

        public static IServiceCollection AddServicesAndRepositories(this IServiceCollection services)
        {
            services.Scan(scan => scan
           .FromAssemblyOf<EventService>()
           .AddClasses(classes => classes.Where(x => x.Name.EndsWith("Service")))
           .AsImplementedInterfaces()
           .WithScopedLifetime());

            services.Scan(scan => scan
           .FromAssemblyOf<CategoryRepository>()
           .AddClasses(classes => classes.Where(x => x.Name.EndsWith("Repository")))
           .AsImplementedInterfaces()
           .WithScopedLifetime());

            return services;
        }
    }
}
