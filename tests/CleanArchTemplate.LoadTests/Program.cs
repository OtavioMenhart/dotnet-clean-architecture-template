using CleanArchTemplate.Application.UseCases.Product.CreateProduct;
using CleanArchTemplate.Application.UseCases.Product.UpdateProduct;
using NBomber.Contracts.Stats;
using NBomber.CSharp;
using System.Text;
using System.Text.Json;

var httpClient = new HttpClient();
var baseUrl = "https://localhost:7121/api/products";

// Wait for the API to be ready
await Task.Delay(5000);

async Task<Guid> CreateProductAsync()
{
    var product = new CreateProductInput("TestProduct", 99.99);
    var json = JsonSerializer.Serialize(product);
    var content = new StringContent(json, Encoding.UTF8, "application/json");
    var response = await httpClient.PostAsync(baseUrl, content);
    var responseBody = await response.Content.ReadAsStringAsync();

    try
    {
        var doc = JsonDocument.Parse(responseBody);
        if (doc.RootElement.TryGetProperty("data", out var dataElem) &&
            dataElem.TryGetProperty("id", out var idElem))
        {
            return idElem.GetGuid();
        }
    }
    catch { }
    return Guid.Empty;
}

async Task<HttpResponseMessage> GetProductByIdAsync(Guid productId)
    => await httpClient.GetAsync($"{baseUrl}/{productId}");

async Task<HttpResponseMessage> UpdateProductAsync(Guid productId)
{
    var update = new UpdateProductInput("UpdatedProduct", 199.99);
    var json = JsonSerializer.Serialize(update);
    var content = new StringContent(json, Encoding.UTF8, "application/json");
    return await httpClient.PutAsync($"{baseUrl}/{productId}", content);
}

async Task<HttpResponseMessage> DeleteProductAsync(Guid productId)
    => await httpClient.DeleteAsync($"{baseUrl}/{productId}");

async Task<HttpResponseMessage> GetAllProductsAsync()
    => await httpClient.GetAsync($"{baseUrl}?pageNumber=1&pageSize=10");

var scenario = Scenario.Create("product_api_load_test", async context =>
{
    // 1. Create product and get Id
    var productId = await CreateProductAsync();

    // 2. Get product by ID
    var getByIdResponse = await GetProductByIdAsync(productId);
    var getByIdBody = await getByIdResponse.Content.ReadAsStringAsync();

    // 3. Update product
    var updateResponse = await UpdateProductAsync(productId);
    var updateBody = await updateResponse.Content.ReadAsStringAsync();

    // 4. Delete product
    var deleteResponse = await DeleteProductAsync(productId);
    var deleteBody = await deleteResponse.Content.ReadAsStringAsync();

    // 5. Get all products
    var getAllResponse = await GetAllProductsAsync();
    var getAllBody = await getAllResponse.Content.ReadAsStringAsync();

    // Returns the result of the last step (GET ALL)
    return Response.Ok(getAllBody, (int)getAllResponse.StatusCode);
})
.WithLoadSimulations(Simulation.Inject(rate: 10, interval: TimeSpan.FromSeconds(1), during: TimeSpan.FromSeconds(30)));

NBomberRunner
    .RegisterScenarios(scenario)
    .WithReportFormats(ReportFormat.Html)
    .Run();