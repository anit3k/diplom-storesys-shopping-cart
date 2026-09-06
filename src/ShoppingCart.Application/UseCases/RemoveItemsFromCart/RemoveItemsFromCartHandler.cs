using ShoppingCart.Application.Ports;
using ShoppingCart.Domain;

namespace ShoppingCart.Application.UseCases.RemoveItemsFromCart;

public class RemoveItemsFromCartHandler
{
    private readonly ICartRepository cartRepository;
    private readonly IEventPublisher eventPublisher;

    public RemoveItemsFromCartHandler(
        ICartRepository cartRepository,
        IEventPublisher eventPublisher)
    {
        this.cartRepository = cartRepository;
        this.eventPublisher = eventPublisher;
    }

    public Cart Handle(RemoveItemsFromCartCommand command)
    {
        var cart = this.cartRepository.Get(command.UserId);

        cart.RemoveItems(command.ProductCatalogueIds);

        this.cartRepository.Save(cart);

        foreach (var productId in command.ProductCatalogueIds)
            this.eventPublisher.Publish("CartItemRemoved", new { command.UserId, productId });

        return cart;
    }
}