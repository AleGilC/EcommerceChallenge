using Microsoft.EntityFrameworkCore;
using ProductService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductService.Infrastructure.Data
{
    public class ProductDbContext(DbContextOptions<ProductDbContext> options) : DbContext(options)
    {
        public DbSet<Product> Products => Set<Product>();
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>(e =>
            {
                e.Property(p => p.Name).IsRequired().HasMaxLength(200);
                e.Property(p => p.Price).HasPrecision(18, 2);
                e.HasIndex(p => p.Name);
            });
        }
    }
}
