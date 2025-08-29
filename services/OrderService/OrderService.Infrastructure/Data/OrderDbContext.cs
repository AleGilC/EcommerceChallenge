using Microsoft.EntityFrameworkCore;
using OrderService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderService.Infrastructure.Data
{
    public class OrderDbContext(DbContextOptions<OrderDbContext> options) : DbContext(options)
    {
        public DbSet<Order> Orders => Set<Order>();
        public DbSet<OrderItem> OrderItems => Set<OrderItem>();
        protected override void OnModelCreating(ModelBuilder b)
        {
            b.Entity<Order>(e =>
            {
                e.OwnsMany(o => o.Items, oi =>
                {
                    oi.WithOwner().HasForeignKey("OrderId");
                    oi.Property<int>("OrderId");
                    oi.ToTable("OrderItems");
                });
            });
        }
    }
}
