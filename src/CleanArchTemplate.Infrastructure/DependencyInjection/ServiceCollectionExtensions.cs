using CleanArchTemplate.Domain.Repositories;
using CleanArchTemplate.Infrastructure.Messaging;
using CleanArchTemplate.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RabbitMq.Messaging.DependencyInjection;

namespace CleanArchTemplate.Infrastructure.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(connectionString));
            return services;
        }

        public static IServiceCollection RegisterRepositories(this IServiceCollection services)
        {
            services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));
            return services;
        }

        public static IServiceCollection RegisterRabbitMq(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddRabbitMqInfrastructure(configuration);
            return services;
        }

        public static IServiceCollection RegisterRabbitMqPublisher(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddRabbitMqPublisher();
            services.AddSingleton<IMessagingService, MessagingService>();
            return services;
        }
    }
}
