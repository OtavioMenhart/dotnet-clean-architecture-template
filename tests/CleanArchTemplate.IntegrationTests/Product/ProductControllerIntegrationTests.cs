using CleanArchTemplate.Api.Responses;
using CleanArchTemplate.Application.UseCases.Product.Common;
using CleanArchTemplate.Application.UseCases.Product.CreateProduct;
using CleanArchTemplate.Application.UseCases.Product.UpdateProduct;
using CleanArchTemplate.Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;
using Testcontainers.RabbitMq;

namespace CleanArchTemplate.IntegrationTests.Product
{
    public class ProductControllerIntegrationTests : IAsyncLifetime
    {
        private readonly RabbitMqContainer _rabbitMqContainer;
        private readonly WebApplicationFactory<Program> _factory;
        private HttpClient _client;

        public ProductControllerIntegrationTests()
        {
            _rabbitMqContainer = new RabbitMqBuilder()
                .WithUsername("guest")
                .WithPassword("guest")
                .Build();

            _factory = new WebApplicationFactory<Program>()
                .WithWebHostBuilder(builder =>
                {
                    builder.UseEnvironment("Testing");
                    builder.ConfigureServices(services =>
                    {
                        // Remove all AppDbContext registrations
                        var dbContextDescriptors = services
                            .Where(d => d.ServiceType == typeof(AppDbContext) || d.ServiceType == typeof(DbContextOptions<AppDbContext>))
                            .ToList();
                        foreach (var descriptor in dbContextDescriptors)
                            services.Remove(descriptor);

                        // Register InMemory database for AppDbContext
                        services.AddDbContext<AppDbContext>(options =>
                        {
                            options.UseInMemoryDatabase("TestDb");
                        });
                    });

                    builder.ConfigureAppConfiguration((context, config) =>
                    {
                        // Inject RabbitMQ connection settings for the test container
                        config.AddInMemoryCollection(new Dictionary<string, string>
                        {
                            ["RabbitMq:Hosts:0"] = _rabbitMqContainer.Hostname,
                            ["RabbitMq:Port"] = _rabbitMqContainer.GetMappedPublicPort(5672).ToString(),
                            ["RabbitMq:Username"] = "guest",
                            ["RabbitMq:Password"] = "guest"
                        });
                    });
                });
        }

        public async Task InitializeAsync()
        {
            await _rabbitMqContainer.StartAsync();
            _client = _factory.CreateClient();
        }

        public async Task DisposeAsync()
        {
            await _rabbitMqContainer.DisposeAsync();
            _client.Dispose();
            _factory.Dispose();
        }

        [Fact]
        public async Task CreateProduct_ReturnsCreated()
        {
            var input = new CreateProductInput("Test Product", 10.99);

            var response = await _client.PostAsJsonAsync("/api/products", input);
            var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<ProductOutput>>();

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            Assert.NotNull(apiResponse);
            Assert.Equal("Test Product", apiResponse.Data.Name);
        }

        [Fact]
        public async Task GetProductById_ReturnsProduct()
        {
            var input = new CreateProductInput("Test", 5.0);
            var createResponse = await _client.PostAsJsonAsync("/api/products", input);
            var created = await createResponse.Content.ReadFromJsonAsync<ApiResponse<ProductOutput>>();

            var response = await _client.GetAsync($"/api/products/{created.Data.Id}");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<ProductOutput>>();
            Assert.Equal("Test", apiResponse.Data.Name);
        }

        [Fact]
        public async Task GetProductById_ReturnsNotFound_ForInvalidId()
        {
            var response = await _client.GetAsync($"/api/products/{Guid.NewGuid()}");
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task GetAllProducts_ReturnsPaginatedList()
        {
            // Create multiple products
            for (int i = 1; i <= 3; i++)
            {
                var input = new CreateProductInput($"Product {i}", 10.0 + i);
                await _client.PostAsJsonAsync("/api/products", input);
            }

            var response = await _client.GetAsync("/api/products?pageNumber=1&pageSize=2");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponseList<ProductOutput>>();
            Assert.NotNull(apiResponse);
            Assert.True(apiResponse.Data.Count() <= 2);
            Assert.True(apiResponse.TotalCount >= 3);
        }

        [Fact]
        public async Task GetAllProducts_ReturnsNotFound_WhenNoProducts()
        {
            var response = await _client.GetAsync("/api/products?pageNumber=100&pageSize=10");
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task UpdateProduct_ReturnsUpdatedProduct()
        {
            var createInput = new CreateProductInput("Old Name", 5.0);
            var createResponse = await _client.PostAsJsonAsync("/api/products", createInput);
            var created = await createResponse.Content.ReadFromJsonAsync<ApiResponse<ProductOutput>>();

            var updateInput = new UpdateProductInput("New Name", 15.0);
            var response = await _client.PutAsJsonAsync($"/api/products/{created.Data.Id}", updateInput);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<ProductOutput>>();
            Assert.Equal("New Name", apiResponse.Data.Name);
            Assert.Equal(15.0, apiResponse.Data.UnitPrice);
        }

        [Fact]
        public async Task UpdateProduct_ReturnsNotFound_ForInvalidId()
        {
            var updateInput = new UpdateProductInput("Name", 10.0);
            var response = await _client.PutAsJsonAsync($"/api/products/{Guid.NewGuid()}", updateInput);
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task DeleteProduct_ReturnsNoContent()
        {
            var createInput = new CreateProductInput("To Delete", 5.0);
            var createResponse = await _client.PostAsJsonAsync("/api/products", createInput);
            var created = await createResponse.Content.ReadFromJsonAsync<ApiResponse<ProductOutput>>();

            var response = await _client.DeleteAsync($"/api/products/{created.Data.Id}");
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }

        [Fact]
        public async Task DeleteProduct_ReturnsNotFound_ForInvalidId()
        {
            var response = await _client.DeleteAsync($"/api/products/{Guid.NewGuid()}");
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}
