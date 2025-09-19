using CleanArchTemplate.Api.Filters;
using CleanArchTemplate.Api.Middlewares;
using CleanArchTemplate.Infrastructure.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
    .AddEnvironmentVariables();

// Add services to the container.
// Database
builder.Services.AddDatabase(builder.Configuration);
builder.Services.RegisterRepositories();

// RabbitMQ
builder.Services.RegisterRabbitMq(builder.Configuration);
builder.Services.RegisterRabbitMqPublisher(builder.Configuration);

builder.Services.AddControllers(options =>
{
    options.Filters.Add<GlobalExceptionFilter>();
});
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();
app.UseMiddleware<ClaimsValidationMiddleware>();

app.MapControllers();

app.Run();
