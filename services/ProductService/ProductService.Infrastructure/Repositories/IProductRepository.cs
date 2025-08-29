using ProductService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductService.Infrastructure.Repositories
{
    public interface IProductRepository
    {
        Task<(IReadOnlyList<Product> Items, int Total)> ListAsync(int page, int size, CancellationToken ct);
        Task<Product?> GetByIdAsync(int id, CancellationToken ct);
        Task<Product> AddAsync(Product entity, CancellationToken ct);
        Task UpdateAsync(Product entity, CancellationToken ct);
        Task DeleteAsync(Product entity, CancellationToken ct);
    }
}
