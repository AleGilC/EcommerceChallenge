using MediatR;
using ProductService.Application.Dtos;
using ProductService.Domain.Entities;
using ProductService.Infrastructure.Repositories;

namespace ProductService.Application.Products.Commands;

public record UpdateProductCommand(int Id, ProductUpdateDto Dto) : IRequest<Product?>;

public class UpdateProductHandler(IProductRepository repo)
    : IRequestHandler<UpdateProductCommand, Product?>
{
    public async Task<Product?> Handle(UpdateProductCommand request, CancellationToken ct)
    {
        var entity = await repo.GetByIdAsync(request.Id, ct);
        if (entity is null) return null;

        entity.Name = request.Dto.Name;
        entity.Description = request.Dto.Description;
        entity.Price = request.Dto.Price;
        entity.Stock = request.Dto.Stock;

        await repo.UpdateAsync(entity, ct);
        return entity;
    }
}
