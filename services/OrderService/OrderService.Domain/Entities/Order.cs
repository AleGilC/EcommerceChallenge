using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderService.Domain.Entities
{
    public enum OrderStatus { Pending, Confirmed, Processing, Shipped, Delivered, Cancelled }
    public class Order
    {
        public int Id { get; set; }
        public string CustomerId { get; set; } = default!;
        public OrderStatus Status { get; set; } = OrderStatus.Pending;
        public decimal TotalAmount { get; set; }
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;
        public List<OrderItem> Items { get; set; } = new();
    }


    public class OrderItem
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }
}
