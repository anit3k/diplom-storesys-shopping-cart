using ShoppingCart.Domain;

namespace ShoppingCart.Application.Ports;

public interface ICartRepository
{
    Cart Get(int userId);
    void Save(Cart cart);
}