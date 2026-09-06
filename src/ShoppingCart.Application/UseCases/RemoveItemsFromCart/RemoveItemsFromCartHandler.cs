using ShoppingCart.Application.Ports;
using ShoppingCart.Domain;

namespace ShoppingCart.Application.UseCases.RemoveItemsFromCart;

public class RemoveItemsFromCartHandler
{
    private readonly ICartRepository cartRepository;
    private readonly IEventStore _eventStore;

    public RemoveItemsFromCartHandler(
        ICartRepository cartRepository,
        IEventStore eventStore)
    {
        this.cartRepository = cartRepository;
        this._eventStore = eventStore;
    }

    public Cart Handle(RemoveItemsFromCartCommand command)
    {
        var cart = this.cartRepository.Get(command.UserId);

        cart.RemoveItems(command.ProductCatalogueIds);

        this.cartRepository.Save(cart);

        foreach (var productId in command.ProductCatalogueIds)
            this._eventStore.Append("CartItemRemoved", new { command.UserId, productId });

        return cart;
    }
}