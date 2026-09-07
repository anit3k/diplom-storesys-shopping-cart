using System.Net.Http.Headers;
using System.Text.Json;

namespace ShoppingCart;

public class ProductCatalogClient
{
    private const string ProductCatalogUrl =
        "https://gist.githubusercontent.com/anit3k/cf11fd86dce483e3963f13d5d30122ae/raw/products.json";

    private readonly HttpClient client;

    public ProductCatalogClient(HttpClient client)
    {
        client.BaseAddress = new Uri(ProductCatalogUrl);
        client.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));

        this.client = client;
    }

    public async Task<IEnumerable<ShoppingCartItem>> GetCartItems(int[] productCatalogIds)
    {
        var response = await this.client.GetAsync(string.Empty);
        response.EnsureSuccessStatusCode();

        var products = await JsonSerializer.DeserializeAsync<List<ProductCatalogProduct>>(
            await response.Content.ReadAsStreamAsync(),
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();

        return products
            .Where(p => productCatalogIds.Contains(p.ProductId))
            .Select(p => new ShoppingCartItem(
                p.ProductId,
                p.ProductName,
                p.ProductDescription,
                p.Price));
    }

    private record ProductCatalogProduct(
        int ProductId,
        string ProductName,
        string ProductDescription,
        Money Price);
}
