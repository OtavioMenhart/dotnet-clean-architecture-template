using CleanArchTemplate.Domain.Repositories;
using CleanArchTemplate.Infrastructure.Messaging;
using CleanArchTemplate.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
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

            services.AddScoped<IUnitOfWork, UnitOfWork>();
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

        public static IHostBuilder MigrateDatabase(this IHostBuilder hostBuilder)
        {
            hostBuilder.ConfigureServices((context, services) =>
            {
                using var scope = services.BuildServiceProvider().CreateScope();
                var logger = scope.ServiceProvider.GetRequiredService<ILogger<AppDbContext>>();
                try
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                    dbContext.Database.Migrate();
                    logger.LogInformation("Database migration applied successfully.");
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "An error occurred while migrating the database.");
                    throw;
                }
            });
            return hostBuilder;
        }
    }
}
