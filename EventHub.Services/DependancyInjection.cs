

namespace EventHub.Services
{
    using EventHub.Repositories.Repositories;
    using EventHub.Services.Services;
    using Microsoft.Extensions.DependencyInjection;
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
