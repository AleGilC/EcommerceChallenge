using ProductService.Application.Dtos;
using ProductService.Domain.Entities;
using ProductService.Infrastructure.Repositories;

namespace ProductService.Application.Services
{
    public interface IProductService
    {
        Task<PagedResult<Product>> ListAsync(int page, int size, CancellationToken ct);
        Task<Product?> GetAsync(int id, CancellationToken ct);
        Task<Product> CreateAsync(ProductCreateDto dto, CancellationToken ct);
        Task<Product?> UpdateAsync(int id, ProductUpdateDto dto, CancellationToken ct);
        Task<bool> DeleteAsync(int id, CancellationToken ct);
    }

    public class ProductService(IProductRepository repo) : IProductService
    {
        public async Task<PagedResult<Product>> ListAsync(int page, int size, CancellationToken ct)
        {
            var (items, total) = await repo.ListAsync(page, size, ct);
            return new(items, page, size, total);
        }


        public Task<Product?> GetAsync(int id, CancellationToken ct) => repo.GetByIdAsync(id, ct);


        public Task<Product> CreateAsync(ProductCreateDto dto, CancellationToken ct)
        => repo.AddAsync(new Product
        {
            Name = dto.Name,
            Description = dto.Description,
            Price = dto.Price,
            Stock = dto.Stock
        }, ct);


        public async Task<Product?> UpdateAsync(int id, ProductUpdateDto dto, CancellationToken ct)
        {
            var entity = await repo.GetByIdAsync(id, ct);
            if (entity is null) return null;
            entity.Name = dto.Name;
            entity.Description = dto.Description;
            entity.Price = dto.Price;
            entity.Stock = dto.Stock;
            await repo.UpdateAsync(entity, ct);
            return entity;
        }


        public async Task<bool> DeleteAsync(int id, CancellationToken ct)
        {
            var entity = await repo.GetByIdAsync(id, ct);
            if (entity is null) return false;
            await repo.DeleteAsync(entity, ct);
            return true;
        }
    }
}
