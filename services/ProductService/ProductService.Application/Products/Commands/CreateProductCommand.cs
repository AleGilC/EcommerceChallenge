using MediatR;
using ProductService.Application.Dtos;
using ProductService.Domain.Entities;
using ProductService.Infrastructure.Repositories;

public record CreateProductCommand(ProductCreateDto Dto) : IRequest<Product>;

public class CreateProductHandler(IProductRepository repo) : IRequestHandler<CreateProductCommand, Product>
{
    public Task<Product> Handle(CreateProductCommand cmd, CancellationToken ct) =>
        repo.AddAsync(new Product
        {
            Name = cmd.Dto.Name,
            Description = cmd.Dto.Description,
            Price = cmd.Dto.Price,
            Stock = cmd.Dto.Stock
        }, ct);
}
