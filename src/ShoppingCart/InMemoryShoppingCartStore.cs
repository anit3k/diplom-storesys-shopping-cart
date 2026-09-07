using System.Collections.Concurrent;

namespace ShoppingCart;

public class InMemoryShoppingCartStore : IShoppingCartStore
{
    private readonly ConcurrentDictionary<int, ShoppingCart> carts = new();

    public ShoppingCart Get(int userId) =>
        this.carts.GetOrAdd(userId, id => new ShoppingCart(id));

    public void Save(ShoppingCart shoppingCart) =>
        this.carts[shoppingCart.UserId] = shoppingCart;
}
