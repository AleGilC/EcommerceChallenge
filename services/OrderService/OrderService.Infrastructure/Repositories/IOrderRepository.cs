using OrderService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderService.Infrastructure.Repositories
{
    public interface IOrderRepository
    {
        Task<(IReadOnlyList<Order> Items, int Total)> ListAsync(int page, int size, CancellationToken ct);
        Task<Order?> GetByIdAsync(int id, CancellationToken ct);
        Task<Order> AddAsync(Order entity, CancellationToken ct);
        Task UpdateAsync(Order entity, CancellationToken ct);
    }
}
