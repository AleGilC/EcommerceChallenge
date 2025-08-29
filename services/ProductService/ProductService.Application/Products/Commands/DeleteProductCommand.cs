using MediatR;
using ProductService.Infrastructure.Repositories;

namespace ProductService.Application.Products.Commands;

public record DeleteProductCommand(int Id) : IRequest<bool>;

public class DeleteProductHandler(IProductRepository repo)
    : IRequestHandler<DeleteProductCommand, bool>
{
    public async Task<bool> Handle(DeleteProductCommand request, CancellationToken ct)
    {
        var entity = await repo.GetByIdAsync(request.Id, ct);
        if (entity is null) return false;

        await repo.DeleteAsync(entity, ct);
        return true;
    }
}
