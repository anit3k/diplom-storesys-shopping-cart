using System.Collections.Concurrent;
using ShoppingCart.Application.Ports;
using ShoppingCart.Domain;

namespace ShoppingCart.Infrastructure;

public class InMemoryCartRepository : ICartRepository
{
    private readonly ConcurrentDictionary<int, Cart> carts = new();

    public Cart Get(int userId) =>
        this.carts.GetOrAdd(userId, id => new Cart(id));

    public void Save(Cart cart) =>
        this.carts[cart.UserId] = cart;
}