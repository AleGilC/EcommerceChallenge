using Microsoft.EntityFrameworkCore;
using ProductService.Domain.Entities;
using ProductService.Infrastructure.Data;

namespace ProductService.Infrastructure.Repositories
{
    public class ProductRepository(ProductDbContext db) : IProductRepository
    {
        public async Task<(IReadOnlyList<Product> Items, int Total)> ListAsync(int page, int size, CancellationToken ct)
        {
            var query = db.Products.AsNoTracking().OrderBy(p => p.Id);
            var total = await query.CountAsync(ct);
            var items = await query.Skip((page - 1) * size).Take(size).ToListAsync(ct);
            return (items, total);
        }
        public Task<Product?> GetByIdAsync(int id, CancellationToken ct) =>
        db.Products.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id, ct);


        public async Task<Product> AddAsync(Product entity, CancellationToken ct)
        {
            entity.CreatedAt = entity.UpdatedAt = DateTime.UtcNow;
            db.Products.Add(entity);
            await db.SaveChangesAsync(ct);
            return entity;
        }
        public async Task UpdateAsync(Product entity, CancellationToken ct)
        {
            entity.UpdatedAt = DateTime.UtcNow;
            db.Products.Update(entity);
            await db.SaveChangesAsync(ct);
        }
        public async Task DeleteAsync(Product entity, CancellationToken ct)
        {
            db.Products.Remove(entity);
            await db.SaveChangesAsync(ct);
        }
    }
}
