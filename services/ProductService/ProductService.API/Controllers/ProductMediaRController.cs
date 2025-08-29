using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using ProductService.Application.Dtos;
using ProductService.Application.Products.Commands;
using ProductService.Application.Products.Queries;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IValidator<ProductCreateDto> _createValidator;
    private readonly IValidator<ProductUpdateDto> _updateValidator;

    public ProductsController(
        IMediator mediator,
        IValidator<ProductCreateDto> createValidator,
        IValidator<ProductUpdateDto> updateValidator)
    {
        _mediator = mediator;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    // GET /api/products?page=1&pageSize=10
    [HttpGet]
    public async Task<IActionResult> List([FromQuery] int page = 1, [FromQuery] int pageSize = 10, CancellationToken ct = default)
    {
        var result = await _mediator.Send(new ListProductsQuery(page, pageSize), ct);
        return Ok(result);
    }

    // GET /api/products/{id}
    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int id, CancellationToken ct)
    {
        var product = await _mediator.Send(new GetProductByIdQuery(id), ct);
        if (product is null) return NotFound();
        return Ok(product);
    }

    // POST /api/products
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] ProductCreateDto dto, CancellationToken ct)
    {
        var validation = await _createValidator.ValidateAsync(dto, ct);
        if (!validation.IsValid)
            return BadRequest(validation.Errors.Select(e => new { e.PropertyName, e.ErrorMessage }));

        var created = await _mediator.Send(new CreateProductCommand(dto), ct);
        return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
    }

    // PUT /api/products/{id}
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] ProductUpdateDto dto, CancellationToken ct)
    {
        var validation = await _updateValidator.ValidateAsync(dto, ct);
        if (!validation.IsValid)
            return BadRequest(validation.Errors.Select(e => new { e.PropertyName, e.ErrorMessage }));

        var updated = await _mediator.Send(new UpdateProductCommand(id, dto), ct);
        if (updated is null) return NotFound();
        return Ok(updated);
    }

    // DELETE /api/products/{id}
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        var ok = await _mediator.Send(new DeleteProductCommand(id), ct);
        return ok ? NoContent() : NotFound();
    }
}
