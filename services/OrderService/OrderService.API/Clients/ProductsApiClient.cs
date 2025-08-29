namespace OrderService.API.Clients
{
    public class ProductsApiClient(IHttpClientFactory factory) : IProductsApiClient
    {
        private readonly HttpClient _http = factory.CreateClient("products");

        public async Task<ProductDto?> GetAsync(int id, CancellationToken ct)
        {
            var resp = await _http.GetAsync($"/api/products/{id}", ct);
            if (resp.StatusCode == System.Net.HttpStatusCode.NotFound) return null;
            resp.EnsureSuccessStatusCode();
            return await resp.Content.ReadFromJsonAsync<ProductDto>(cancellationToken: ct);
        }
    }
}
