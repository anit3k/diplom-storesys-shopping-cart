using ShoppingCart.Application.Ports;
using ShoppingCart.Domain;

namespace ShoppingCart.Infrastructure;

public class FakeProductCatalogClient : IProductCatalogClient
{
    public Task<IEnumerable<CartItem>> GetCartItems(int[] productCatalogIds)
    {
        var items = productCatalogIds.Select(id => new CartItem(
            id,
            $"Product {id}",
            "A placeholder product description",
            new Money("DKK", 99.95m)));

        return Task.FromResult(items);
    }
}