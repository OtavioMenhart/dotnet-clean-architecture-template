using CleanArchTemplate.Api.Filters;
using CleanArchTemplate.Api.Middlewares;
using CleanArchTemplate.Application.DependencyInjection;
using CleanArchTemplate.Infrastructure.DependencyInjection;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
    .AddEnvironmentVariables();

// Serilog
builder.Host.UseSerilog((context, services, configuration) =>
    configuration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
);

// OpenTelemetry
if (!builder.Environment.IsEnvironment("Testing"))
{
    builder.Services.RegisterOpenTelemetry(builder.Configuration, "Api");

    // Database
    builder.Services.AddDatabase(builder.Configuration);
}

// Add services to the container.
builder.Services.RegisterRepositories();

// RabbitMQ
builder.Services.RegisterRabbitMq(builder.Configuration);
builder.Services.RegisterRabbitMqPublisher(builder.Configuration);

// Handlers
builder.Services.AddApplicationHandlers();

builder.Services.AddControllers(options =>
{
    options.Filters.Add<GlobalExceptionFilter>();
});

// Swagger configuration
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "CleanArchTemplate", Version = "v1" });

    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    options.IncludeXmlComments(xmlPath);
});

// Database migration
if (!builder.Environment.IsEnvironment("Testing"))
    builder.Host.MigrateDatabase();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();
app.UseMiddleware<ClaimsValidationMiddleware>();

app.MapControllers();
app.UseSerilogRequestLogging();
await app.RunAsync();

public partial class Program { }