using ShoppingCart.Domain;

namespace ShoppingCart.Application.Ports;

public interface IProductCatalogClient
{
    Task<IEnumerable<CartItem>> GetCartItems(int[] productCatalogIds);
}