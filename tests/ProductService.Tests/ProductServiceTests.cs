using FluentAssertions;
using Moq;
using ProductService.Application.Dtos;
using ProductService.Application.Services;
using ProductService.Domain.Entities;
using ProductService.Infrastructure.Repositories;


public class ProductServiceTests
{
    [Fact]
    public async Task CreateProduct_WithValidData_ReturnsCreated()
    {
        var repo = new Mock<IProductRepository>();
        repo.Setup(r => r.AddAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()))
        .ReturnsAsync((Product p, CancellationToken _) => { p.Id = 1; return p; });


        var svc = new ProductService.Application.Services.ProductService(repo.Object);
        var dto = new ProductCreateDto("Test", "Desc", 10m, 5);


        var created = await svc.CreateAsync(dto, default);


        created.Should().NotBeNull();
        created.Name.Should().Be("Test");
        repo.Verify(r => r.AddAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}