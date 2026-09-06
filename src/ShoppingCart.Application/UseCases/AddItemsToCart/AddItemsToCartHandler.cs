using ShoppingCart.Application.Ports;
using ShoppingCart.Domain;

namespace ShoppingCart.Application.UseCases.AddItemsToCart;

public class AddItemsToCartHandler
{
    private readonly ICartRepository cartRepository;
    private readonly IProductCatalogClient productCatalogClient;
    private readonly IEventStore _eventStore;

    public AddItemsToCartHandler(
        ICartRepository cartRepository,
        IProductCatalogClient productCatalogClient,
        IEventStore eventStore)
    {
        this.cartRepository = cartRepository;
        this.productCatalogClient = productCatalogClient;
        this._eventStore = eventStore;
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
            this._eventStore.Append("CartItemAdded", new { command.UserId, item });

        return cart;
    }
}