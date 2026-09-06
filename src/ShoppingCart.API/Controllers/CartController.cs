using Microsoft.AspNetCore.Mvc;
using ShoppingCart.Application.UseCases.AddItemsToCart;
using ShoppingCart.Application.UseCases.GetCart;
using ShoppingCart.Application.UseCases.RemoveItemsFromCart;

namespace ShoppingCart.API.Controllers;

[ApiController]
[Route("cart")]
public class CartController : ControllerBase
{
    private readonly GetCartHandler getCartHandler;
    private readonly AddItemsToCartHandler addItemsToCartHandler;
    private readonly RemoveItemsFromCartHandler removeItemsFromCartHandler;

    public CartController(
        GetCartHandler getCartHandler,
        AddItemsToCartHandler addItemsToCartHandler,
        RemoveItemsFromCartHandler removeItemsFromCartHandler)
    {
        this.getCartHandler = getCartHandler;
        this.addItemsToCartHandler = addItemsToCartHandler;
        this.removeItemsFromCartHandler = removeItemsFromCartHandler;
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
}