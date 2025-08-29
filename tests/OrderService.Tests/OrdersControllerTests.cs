using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using OrderService.API.Clients;
using OrderService.API.Controllers;
using OrderService.Application.Dtos;
using OrderService.Application.Services;


public class OrdersControllerTests
{
    [Fact]
    public async Task Create_Validates_Product_Existence_And_Stock()
    {
        var svc = new Mock<IOrderService>();
        svc.Setup(s => s.CreateAsync(It.IsAny<OrderCreateDto>(), It.IsAny<CancellationToken>()))
        .ReturnsAsync(new OrderService.Domain.Entities.Order { Id = 123 });


        var products = new Mock<IProductsApiClient>();
        products.Setup(p => p.GetAsync(1, It.IsAny<CancellationToken>()))
        .ReturnsAsync(new ProductDto(1, "P", "D", 10m, 5));


        var controller = new OrdersController(svc.Object, products.Object);


        var dto = new OrderCreateDto("cust-1", new() { new(1, 2, 10m) });
        var result = await controller.Create(dto, default);


        result.Result.Should().BeOfType<CreatedAtActionResult>();
    }
}