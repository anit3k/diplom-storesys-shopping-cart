namespace ShoppingCart.Domain;

public record CartItem(
    int ProductCatalogueId,
    string ProductName,
    string Description,
    Money Price)
{
    public virtual bool Equals(CartItem? obj) =>
        obj != null && this.ProductCatalogueId.Equals(obj.ProductCatalogueId);

    public override int GetHashCode() => this.ProductCatalogueId.GetHashCode();
}