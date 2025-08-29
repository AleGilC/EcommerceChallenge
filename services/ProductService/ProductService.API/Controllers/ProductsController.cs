using Microsoft.AspNetCore.Mvc;
using ProductService.Application.Dtos;
using ProductService.Application.Services;

namespace ProductService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController(IProductService service) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<PagedResult<object>>> List([FromQuery] int page = 1, [FromQuery] int pageSize = 10, CancellationToken ct = default)
        {
            var result = await service.ListAsync(page, pageSize, ct);
            var shape = new PagedResult<object>(
            result.Items.Select(p => new { p.Id, p.Name, p.Description, p.Price, p.Stock, p.CreatedAt, p.UpdatedAt }).ToList(),
            result.Page, result.PageSize, result.Total);
            return Ok(shape);
        }


        [HttpGet("{id:int}")]
        public async Task<ActionResult<object>> Get(int id, CancellationToken ct)
        {
            var p = await service.GetAsync(id, ct);
            return p is null ? NotFound() : Ok(new { p.Id, p.Name, p.Description, p.Price, p.Stock, p.CreatedAt, p.UpdatedAt });
        }


        [HttpPost]
        public async Task<ActionResult<object>> Create([FromBody] ProductCreateDto dto, CancellationToken ct)
        {
            var created = await service.CreateAsync(dto, ct);
            return CreatedAtAction(nameof(Get), new { id = created.Id }, new { created.Id, created.Name, created.Description, created.Price, created.Stock, created.CreatedAt, created.UpdatedAt });
        }


        [HttpPut("{id:int}")]
        public async Task<ActionResult<object>> Update(int id, [FromBody] ProductUpdateDto dto, CancellationToken ct)
        {
            var updated = await service.UpdateAsync(id, dto, ct);
            return updated is null ? NotFound() : Ok(new { updated.Id, updated.Name, updated.Description, updated.Price, updated.Stock, updated.CreatedAt, updated.UpdatedAt });
        }


        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
            var ok = await service.DeleteAsync(id, ct);
            return ok ? NoContent() : NotFound();
        }
    }
}
