using Microsoft.EntityFrameworkCore;
using OrderService.Domain.Entities;
using OrderService.Infrastructure.Data;

namespace OrderService.Infrastructure.Repositories
{
    public class OrderRepository(OrderDbContext db) : IOrderRepository
    {
        public async Task<(IReadOnlyList<Order> Items, int Total)> ListAsync(int page, int size, CancellationToken ct)
        {
            var query = db.Orders.AsNoTracking().Include(o => o.Items).OrderByDescending(o => o.OrderDate);
            var total = await query.CountAsync(ct);
            var items = await query.Skip((page - 1) * size).Take(size).ToListAsync(ct);
            return (items, total);
        }
        public Task<Order?> GetByIdAsync(int id, CancellationToken ct) =>
        db.Orders.AsNoTracking().Include(o => o.Items).FirstOrDefaultAsync(o => o.Id == id, ct);


        public async Task<Order> AddAsync(Order entity, CancellationToken ct)
        {
            db.Orders.Add(entity);
            await db.SaveChangesAsync(ct);
            return entity;
        }
        public async Task UpdateAsync(Order entity, CancellationToken ct)
        {
            db.Orders.Update(entity);
            await db.SaveChangesAsync(ct);
        }
    }
}
