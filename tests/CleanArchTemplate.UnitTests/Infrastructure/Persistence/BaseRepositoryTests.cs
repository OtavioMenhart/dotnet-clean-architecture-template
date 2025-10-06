using CleanArchTemplate.Domain.Entities;
using CleanArchTemplate.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CleanArchTemplate.UnitTests.Infrastructure.Persistence;

public class BaseRepositoryTests
{
    private TestDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<TestDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new TestDbContext(options);
    }

    private ProductEntity CreateProduct(string name = "Test", double price = 10.0)
    {
        return new ProductEntity(name, price);
    }

    [Fact]
    public async Task AddAsync_AddsEntity()
    {
        using var context = CreateContext();
        var repo = new BaseRepository<ProductEntity>(context);
        var product = CreateProduct();

        await repo.AddAsync(product, CancellationToken.None);
        await context.SaveChangesAsync();

        var found = await repo.GetByIdAsync(product.Id, CancellationToken.None);
        Assert.NotNull(found);
        Assert.Equal(product.Name, found.Name);
    }

    [Fact]
    public async Task AddRangeAsync_AddsEntities()
    {
        using var context = CreateContext();
        var repo = new BaseRepository<ProductEntity>(context);
        var products = new[] { CreateProduct("A", 1), CreateProduct("B", 2) };

        await repo.AddRangeAsync(products, CancellationToken.None);
        await context.SaveChangesAsync();

        var all = await repo.GetAllAsync(CancellationToken.None);
        Assert.Equal(2, all.Count());
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsEntity()
    {
        using var context = CreateContext();
        var repo = new BaseRepository<ProductEntity>(context);
        var product = CreateProduct();
        await repo.AddAsync(product, CancellationToken.None);
        await context.SaveChangesAsync();

        var found = await repo.GetByIdAsync(product.Id, CancellationToken.None);
        Assert.NotNull(found);
        Assert.Equal(product.Name, found.Name);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllEntities()
    {
        using var context = CreateContext();
        var repo = new BaseRepository<ProductEntity>(context);
        var products = new[] { CreateProduct("A", 1), CreateProduct("B", 2) };
        await repo.AddRangeAsync(products, CancellationToken.None);
        await context.SaveChangesAsync();

        var all = await repo.GetAllAsync(CancellationToken.None);
        Assert.Equal(2, all.Count());
    }

    [Fact]
    public async Task GetPagedAsync_ReturnsPagedEntities()
    {
        using var context = CreateContext();
        var repo = new BaseRepository<ProductEntity>(context);
        for (int i = 1; i <= 15; i++)
            await repo.AddAsync(CreateProduct($"P{i}", i), CancellationToken.None);
        await context.SaveChangesAsync();

        var paged = await repo.GetPagedAsync(2, 5, CancellationToken.None);
        Assert.Equal(5, paged.Count());
        Assert.Equal("P6", paged.First().Name);
    }

    [Fact]
    public async Task Update_UpdatesEntityAndSetsUpdatedAt()
    {
        using var context = CreateContext();
        var repo = new BaseRepository<ProductEntity>(context);
        var product = CreateProduct();
        await repo.AddAsync(product, CancellationToken.None);
        await context.SaveChangesAsync();

        product.ChangeName("Updated");
        repo.Update(product);
        await context.SaveChangesAsync();

        var found = await repo.GetByIdAsync(product.Id, CancellationToken.None);
        Assert.Equal("Updated", found.Name);
        Assert.NotNull(found.UpdatedAt);
    }

    [Fact]
    public async Task DeleteAsync_RemovesEntity()
    {
        using var context = CreateContext();
        var repo = new BaseRepository<ProductEntity>(context);
        var product = CreateProduct();
        await repo.AddAsync(product, CancellationToken.None);
        await context.SaveChangesAsync();

        await repo.DeleteAsync(product.Id, CancellationToken.None);
        await context.SaveChangesAsync();

        var found = await repo.GetByIdAsync(product.Id, CancellationToken.None);
        Assert.Null(found);
    }

    [Fact]
    public async Task ExistsAsync_ReturnsTrueIfExists()
    {
        using var context = CreateContext();
        var repo = new BaseRepository<ProductEntity>(context);
        var product = CreateProduct();
        await repo.AddAsync(product, CancellationToken.None);
        await context.SaveChangesAsync();

        var exists = await repo.ExistsAsync(product.Id, CancellationToken.None);
        Assert.True(exists);
    }

    [Fact]
    public async Task ExistsAsync_ReturnsFalseIfNotExists()
    {
        using var context = CreateContext();
        var repo = new BaseRepository<ProductEntity>(context);

        var exists = await repo.ExistsAsync(Guid.NewGuid(), CancellationToken.None);
        Assert.False(exists);
    }

    [Fact]
    public async Task CountAsync_ReturnsNumberOfEntities()
    {
        using var context = CreateContext();
        var repo = new BaseRepository<ProductEntity>(context);
        var products = new[] { CreateProduct("A", 1), CreateProduct("B", 2) };
        await repo.AddRangeAsync(products, CancellationToken.None);
        await context.SaveChangesAsync();

        var count = await repo.CountAsync(CancellationToken.None);
        Assert.Equal(2, count);
    }
}
