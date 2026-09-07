namespace ShoppingCart;

public class ShoppingCart
{
    private readonly HashSet<ShoppingCartItem> items = new();

    public int UserId { get; }
    public IEnumerable<ShoppingCartItem> Items => this.items;

    public ShoppingCart(int userId) => this.UserId = userId;

    public void AddItems(IEnumerable<ShoppingCartItem> shoppingCartItems, IEventStore eventStore)
    {
        foreach (var item in shoppingCartItems)
        {
            this.items.Add(item);
            eventStore.Append("CartItemAdded", new { this.UserId, item });
        }
    }

    public void RemoveItems(int[] productCatalogueIds, IEventStore eventStore)
    {
        this.items.RemoveWhere(i => productCatalogueIds.Contains(i.ProductCatalogueId));

        foreach (var productId in productCatalogueIds)
            eventStore.Append("CartItemRemoved", new { this.UserId, productId });
    }
}
