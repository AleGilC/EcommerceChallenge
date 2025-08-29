using OrderService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderService.Application.Dtos
{
    public record OrderItemCreateDto(int ProductId, int Quantity, decimal UnitPrice);
    public record OrderCreateDto(string CustomerId, List<OrderItemCreateDto> Items);
    public record OrderStatusUpdateDto(OrderStatus Status);
    public record PagedResult<T>(IReadOnlyList<T> Items, int Page, int PageSize, int Total);
}
