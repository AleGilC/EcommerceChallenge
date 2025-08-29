using System.Net.Http.Json;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Polly;
using Polly.Extensions.Http;
using OrderService.Application.Validation;
using OrderService.Application.Services;
using OrderService.Infrastructure.Data;
using OrderService.Infrastructure.Repositories;
using OrderService.API.Clients;
using MediatR;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// FluentValidation (sin paquete AspNetCore deprecado)
builder.Services.AddValidatorsFromAssemblyContaining<OrderCreateValidator>();

builder.Services.AddDbContext<OrderDbContext>(o =>
    o.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IOrderService, OrderService.Application.Services.OrderService>();
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(OrderService.Application.Dtos.OrderCreateDto).Assembly));

builder.Services.AddHttpClient<IProductsApiClient, ProductsApiClient>(c =>
{
    c.BaseAddress = new Uri(builder.Configuration["ProductService:BaseUrl"]!);
})
.AddPolicyHandler(HttpPolicyExtensions
    .HandleTransientHttpError()
    .WaitAndRetryAsync(3, retry => TimeSpan.FromMilliseconds(200 * (retry + 1))));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Order Service", Version = "v1" });
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

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<OrderDbContext>();
    db.Database.Migrate();
}

app.Map("/error", (HttpContext ctx) =>
    Results.Problem(title: "Unhandled error", statusCode: 500));

app.Run();
