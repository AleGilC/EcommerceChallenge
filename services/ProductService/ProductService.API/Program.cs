using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using ProductService.Application.Validation;
using ProductService.Application.Services;
using ProductService.Infrastructure.Data;
using ProductService.Infrastructure.Repositories;
using MediatR;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// ✅ FluentValidation (sin paquete AspNetCore deprecado)
builder.Services.AddValidatorsFromAssemblyContaining<ProductCreateValidator>();

// EF Core (SQL Server)
builder.Services.AddDbContext<ProductDbContext>(o =>
    o.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

// DI capa de dominio/infra
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IProductService, ProductService.Application.Services.ProductService>();

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(ProductService.Application.Dtos.ProductCreateDto).Assembly));

// Swagger + HealthChecks
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Product Service", Version = "v1" });
});
builder.Services.AddHealthChecks();

var app = builder.Build();

app.UseExceptionHandler("/error");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapHealthChecks("/health");
app.MapControllers();

// Migraciones y seed (solo para demo/dev)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ProductDbContext>();
    db.Database.Migrate();

    if (!db.Products.Any())
    {
        db.Products.AddRange(
            new() { Name = "Mouse", Description = "Optical Mouse", Price = 15, Stock = 100, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new() { Name = "Keyboard", Description = "Mechanical", Price = 50, Stock = 50, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow }
        );
        db.SaveChanges();
    }
}

// Endpoint de error estándar (ProblemDetails)
app.Map("/error", (HttpContext ctx) =>
    Results.Problem(title: "Unhandled error", statusCode: 500));

app.Run();
