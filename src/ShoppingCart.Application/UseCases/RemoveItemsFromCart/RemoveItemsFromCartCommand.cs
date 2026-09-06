namespace ShoppingCart.Application.UseCases.RemoveItemsFromCart;

public record RemoveItemsFromCartCommand(int UserId, int[] ProductCatalogueIds);