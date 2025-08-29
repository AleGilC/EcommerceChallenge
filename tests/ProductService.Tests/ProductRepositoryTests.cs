using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using ProductService.Domain.Entities;
using ProductService.Infrastructure.Data;
using ProductService.Infrastructure.Repositories;


public class ProductRepositoryTests
{
    [Fact]
    public async Task Crud_WithInMemory_Works()
    {
        var options = new DbContextOptionsBuilder<ProductDbContext>()
        .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        using var db = new ProductDbContext(options);
        var repo = new ProductRepository(db);


        var created = await repo.AddAsync(new Product { Name = "N", Description = "D", Price = 1, Stock = 1 }, default);
        created.Id.Should().BeGreaterThan(0);


        var fetched = await repo.GetByIdAsync(created.Id, default);
        fetched!.Name.Should().Be("N");


        fetched.Name = "N2"; await repo.UpdateAsync(fetched, default);
        (await repo.GetByIdAsync(created.Id, default))!.Name.Should().Be("N2");


        await repo.DeleteAsync(fetched, default);
        (await repo.GetByIdAsync(created.Id, default)).Should().BeNull();
    }
}