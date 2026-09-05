namespace ShoppingCart.Domain;

public class Cart
{
    private readonly HashSet<CartItem> items = new();

    public int UserId { get; }
    public IEnumerable<CartItem> Items => this.items;

    public Cart(int userId) => this.UserId = userId;

    public void AddItems(IEnumerable<CartItem> cartItems)
    {
        foreach (var item in cartItems)
            this.items.Add(item);
    }

    public void RemoveItems(int[] productCatalogueIds) =>
        this.items.RemoveWhere(i => productCatalogueIds.Contains(i.ProductCatalogueId));
}