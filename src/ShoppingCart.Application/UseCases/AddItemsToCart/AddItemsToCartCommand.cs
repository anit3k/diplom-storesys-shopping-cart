namespace ShoppingCart.Application.UseCases.AddItemsToCart;

public record AddItemsToCartCommand(int UserId, int[] ProductIds);