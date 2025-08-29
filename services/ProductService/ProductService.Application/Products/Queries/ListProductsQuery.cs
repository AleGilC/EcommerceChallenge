using MediatR;
using ProductService.Application.Dtos;
using ProductService.Domain.Entities;
using ProductService.Infrastructure.Repositories;

public record ListProductsQuery(int Page, int PageSize) : IRequest<PagedResult<Product>>;

public class ListProductsHandler(IProductRepository repo) : IRequestHandler<ListProductsQuery, PagedResult<Product>>
{
    public async Task<PagedResult<Product>> Handle(ListProductsQuery q, CancellationToken ct)
    {
        var (items, total) = await repo.ListAsync(q.Page, q.PageSize, ct);
        return new(items, q.Page, q.PageSize, total);
    }
}
