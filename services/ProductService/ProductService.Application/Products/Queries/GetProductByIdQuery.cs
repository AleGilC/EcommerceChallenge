using MediatR;
using ProductService.Domain.Entities;
using ProductService.Infrastructure.Repositories;

namespace ProductService.Application.Products.Queries;

public record GetProductByIdQuery(int Id) : IRequest<Product?>;

public class GetProductByIdHandler(IProductRepository repo)
    : IRequestHandler<GetProductByIdQuery, Product?>
{
    public Task<Product?> Handle(GetProductByIdQuery request, CancellationToken ct)
        => repo.GetByIdAsync(request.Id, ct);
}
