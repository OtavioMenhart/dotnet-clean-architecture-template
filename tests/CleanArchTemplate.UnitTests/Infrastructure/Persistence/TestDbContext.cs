using CleanArchTemplate.Domain.Entities;
using CleanArchTemplate.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CleanArchTemplate.UnitTests.Infrastructure.Persistence;

public class TestDbContext : AppDbContext
{
    public DbSet<ProductEntity> Products { get; set; }
    public TestDbContext(DbContextOptions options) : base(options) { }
}
