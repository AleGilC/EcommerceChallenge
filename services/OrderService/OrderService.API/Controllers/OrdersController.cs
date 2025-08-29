using Microsoft.AspNetCore.Mvc;
using OrderService.API.Clients;
using OrderService.Application.Dtos;
using OrderService.Application.Services;

namespace OrderService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController(IOrderService service, IProductsApiClient products) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<PagedResult<object>>> List([FromQuery] int page = 1, [FromQuery] int pageSize = 10, CancellationToken ct = default)
        {
            var result = await service.ListAsync(page, pageSize, ct);
            var shape = new PagedResult<object>(
            result.Items.Select(o => new { o.Id, o.CustomerId, o.Status, o.TotalAmount, o.OrderDate, Items = o.Items.Select(i => new { i.ProductId, i.Quantity, i.UnitPrice }) }).ToList(),
            result.Page, result.PageSize, result.Total);
            return Ok(shape);
        }


        [HttpGet("{id:int}")]
        public async Task<ActionResult<object>> Get(int id, CancellationToken ct)
        {
            var o = await service.GetAsync(id, ct);
            return o is null ? NotFound() : Ok(new { o.Id, o.CustomerId, o.Status, o.TotalAmount, o.OrderDate, Items = o.Items.Select(i => new { i.ProductId, i.Quantity, i.UnitPrice }) });
        }


        [HttpPost]
        public async Task<ActionResult<object>> Create([FromBody] OrderCreateDto dto, CancellationToken ct)
        {
            // Reglas de negocio: producto existe + stock suficiente por ítem
            foreach (var item in dto.Items)
            {
                var product = await products.GetAsync(item.ProductId, ct);
                if (product is null) return NotFound(new { message = $"Product {item.ProductId} not found" });
                if (product.Stock < item.Quantity) return BadRequest(new { message = $"Insufficient stock for product {item.ProductId}" });
                if (item.UnitPrice <= 0) return BadRequest(new { message = $"UnitPrice must be > 0 for product {item.ProductId}" });
            }


            var created = await service.CreateAsync(dto, ct);
            return CreatedAtAction(nameof(Get), new { id = created.Id }, new { created.Id });
        }


        [HttpPut("{id:int}/status")]
        public async Task<ActionResult<object>> UpdateStatus(int id, [FromBody] OrderStatusUpdateDto dto, CancellationToken ct)
        {
            var updated = await service.UpdateStatusAsync(id, dto.Status, ct);
            return updated is null ? NotFound() : Ok(new { updated.Id, updated.Status });
        }
    }
}
