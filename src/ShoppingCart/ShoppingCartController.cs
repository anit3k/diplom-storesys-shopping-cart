using Microsoft.AspNetCore.Mvc;

namespace ShoppingCart;

[ApiController]
[Route("cart")]
public class ShoppingCartController : ControllerBase
{
    private readonly IShoppingCartStore shoppingCartStore;
    private readonly ProductCatalogClient productCatalogClient;
    private readonly IEventStore eventStore;

    public ShoppingCartController(
        IShoppingCartStore shoppingCartStore,
        ProductCatalogClient productCatalogClient,
        IEventStore eventStore)
    {
        this.shoppingCartStore = shoppingCartStore;
        this.productCatalogClient = productCatalogClient;
        this.eventStore = eventStore;
    }

    [HttpGet("{userId}")]
    public IActionResult Get(int userId)
    {
        var cart = this.shoppingCartStore.Get(userId);
        return this.Ok(cart);
    }

    [HttpPost("{userId}/items")]
    public async Task<IActionResult> AddItems(int userId, [FromBody] int[] productIds)
    {
        var cart = this.shoppingCartStore.Get(userId);
        var itemsToAdd = await this.productCatalogClient.GetCartItems(productIds);
        cart.AddItems(itemsToAdd, this.eventStore);
        this.shoppingCartStore.Save(cart);
        return this.Ok(cart);
    }

    [HttpDelete("{userId}/items")]
    public IActionResult RemoveItems(int userId, [FromBody] int[] productIds)
    {
        var cart = this.shoppingCartStore.Get(userId);
        cart.RemoveItems(productIds, this.eventStore);
        this.shoppingCartStore.Save(cart);
        return this.Ok(cart);
    }

    [HttpGet("events")]
    public IActionResult GetEvents([FromQuery] long from = 0)
    {
        var events = this.eventStore.GetEventsFrom(from);
        return this.Ok(events);
    }
}
