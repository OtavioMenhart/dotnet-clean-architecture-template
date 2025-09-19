using CleanArchTemplate.Application.Handlers;
using Microsoft.Extensions.DependencyInjection;

namespace CleanArchTemplate.Application.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplicationHandlers(this IServiceCollection services)
        {
            var handlerInterfaceType = typeof(IHandler<,>);
            var assembly = typeof(IHandler<,>).Assembly; // Application assembly

            var handlerTypes = assembly.GetTypes()
                .Where(t => !t.IsAbstract && !t.IsInterface)
                .SelectMany(t => t.GetInterfaces()
                    .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == handlerInterfaceType)
                    .Select(i => new { Handler = t, Interface = i }));

            foreach (var ht in handlerTypes)
            {
                services.AddTransient(ht.Interface, ht.Handler);
            }

            services.AddScoped<IRequestDispatcher, RequestDispatcher>();
            return services;
        }
    }
}
