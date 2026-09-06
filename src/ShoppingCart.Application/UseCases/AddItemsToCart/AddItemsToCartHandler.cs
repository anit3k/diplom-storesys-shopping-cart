using ShoppingCart.Application.Ports;
using ShoppingCart.Domain;

namespace ShoppingCart.Application.UseCases.AddItemsToCart;

public class AddItemsToCartHandler
{
    private readonly ICartRepository cartRepository;
    private readonly IProductCatalogClient productCatalogClient;
    private readonly IEventPublisher eventPublisher;

    public AddItemsToCartHandler(
        ICartRepository cartRepository,
        IProductCatalogClient productCatalogClient,
        IEventPublisher eventPublisher)
    {
        this.cartRepository = cartRepository;
        this.productCatalogClient = productCatalogClient;
        this.eventPublisher = eventPublisher;
    }

    public async Task<Cart> Handle(AddItemsToCartCommand command)
    {
        var cart = this.cartRepository.Get(command.UserId);

        var itemsToAdd = await this.productCatalogClient
            .GetCartItems(command.ProductIds);

        var itemsList = itemsToAdd.ToList();
        cart.AddItems(itemsList);

        this.cartRepository.Save(cart);

        foreach (var item in itemsList)
            this.eventPublisher.Publish("CartItemAdded", new { command.UserId, item });

        return cart;
    }
}