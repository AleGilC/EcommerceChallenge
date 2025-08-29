using OrderService.Application.Dtos;
using OrderService.Domain.Entities;
using OrderService.Infrastructure.Repositories;

namespace OrderService.Application.Services
{
    public interface IOrderService
    {
        Task<PagedResult<Order>> ListAsync(int page, int size, CancellationToken ct);
        Task<Order?> GetAsync(int id, CancellationToken ct);
        Task<Order> CreateAsync(OrderCreateDto dto, CancellationToken ct);
        Task<Order?> UpdateStatusAsync(int id, OrderStatus status, CancellationToken ct);
    }

    public class OrderService(IOrderRepository repo) : IOrderService
    {
        public async Task<PagedResult<Order>> ListAsync(int page, int size, CancellationToken ct)
        {
            var (items, total) = await repo.ListAsync(page, size, ct);
            return new(items, page, size, total);
        }


        public Task<Order?> GetAsync(int id, CancellationToken ct) => repo.GetByIdAsync(id, ct);


        public async Task<Order> CreateAsync(OrderCreateDto dto, CancellationToken ct)
        {
            // Total
            decimal total = dto.Items.Sum(i => i.UnitPrice * i.Quantity);
            var order = new Order
            {
                CustomerId = dto.CustomerId,
                Status = OrderStatus.Pending,
                TotalAmount = total,
                Items = dto.Items.Select(i => new OrderItem
                {
                    ProductId = i.ProductId,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice
                }).ToList()
            };
            return await repo.AddAsync(order, ct);
        }


        public async Task<Order?> UpdateStatusAsync(int id, OrderStatus status, CancellationToken ct)
        {
            var entity = await repo.GetByIdAsync(id, ct);
            if (entity is null) return null;
            entity.Status = status;
            await repo.UpdateAsync(entity, ct);
            return entity;
        }
    }
}
