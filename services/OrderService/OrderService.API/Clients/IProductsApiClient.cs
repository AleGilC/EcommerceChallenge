namespace OrderService.API.Clients
{
    public interface IProductsApiClient
    {
        Task<ProductDto?> GetAsync(int id, CancellationToken ct);
    }

    public record ProductDto(int Id, string Name, string Description, decimal Price, int Stock);
}
