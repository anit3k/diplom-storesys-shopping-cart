using Microsoft.AspNetCore.Mvc;
using ShoppingCart.Application.UseCases.AddItemsToCart;
using ShoppingCart.Application.UseCases.GetCart;
using ShoppingCart.Application.UseCases.GetEvents;
using ShoppingCart.Application.UseCases.RemoveItemsFromCart;

namespace ShoppingCart.API.Controllers;

[ApiController]
[Route("cart")]
public class CartController : ControllerBase
{
    private readonly GetCartHandler getCartHandler;
    private readonly AddItemsToCartHandler addItemsToCartHandler;
    private readonly RemoveItemsFromCartHandler removeItemsFromCartHandler;
    private readonly GetEventsHandler getEventsHandler;

    public CartController(
        GetCartHandler getCartHandler,
        AddItemsToCartHandler addItemsToCartHandler,
        RemoveItemsFromCartHandler removeItemsFromCartHandler,
        GetEventsHandler getEventsHandler)
    {
        this.getCartHandler = getCartHandler;
        this.addItemsToCartHandler = addItemsToCartHandler;
        this.removeItemsFromCartHandler = removeItemsFromCartHandler;
        this.getEventsHandler = getEventsHandler;
    }

    [HttpGet("{userId}")]
    public IActionResult Get(int userId)
    {
        var cart = this.getCartHandler.Handle(new GetCartQuery(userId));
        return this.Ok(cart);
    }

    [HttpPost("{userId}/items")]
    public async Task<IActionResult> AddItems(int userId, [FromBody] int[] productIds)
    {
        var cart = await this.addItemsToCartHandler.Handle(
            new AddItemsToCartCommand(userId, productIds));

        return this.Ok(cart);
    }

    [HttpDelete("{userId}/items")]
    public IActionResult RemoveItems(int userId, [FromBody] int[] productIds)
    {
        var cart = this.removeItemsFromCartHandler.Handle(
            new RemoveItemsFromCartCommand(userId, productIds));

        return this.Ok(cart);
    }
    
    [HttpGet("events")]
    public IActionResult GetEvents([FromQuery] long from = 0)
    {
        var events = this.getEventsHandler.Handle(new GetEventsQuery(from));
        return this.Ok(events);
    }
}