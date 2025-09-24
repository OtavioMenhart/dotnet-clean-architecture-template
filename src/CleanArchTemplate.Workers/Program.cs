using CleanArchTemplate.Infrastructure.DependencyInjection;
using CleanArchTemplate.Workers.Consumers;
using Serilog;

var builder = Host.CreateApplicationBuilder(args);
builder.Configuration
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
    .AddEnvironmentVariables();

// Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();
builder.Services.AddLogging(loggingBuilder =>
{
    loggingBuilder.ClearProviders();
    loggingBuilder.AddSerilog(Log.Logger, true);
});

builder.Services.RegisterRabbitMq(builder.Configuration);
builder.Services.AddHostedService<EntityEventConsumer>();

var host = builder.Build();
host.Run();
