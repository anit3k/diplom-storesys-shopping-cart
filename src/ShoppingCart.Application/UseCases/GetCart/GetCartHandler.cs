using ShoppingCart.Application.Ports;
using ShoppingCart.Domain;

namespace ShoppingCart.Application.UseCases.GetCart;

public class GetCartHandler
{
    private readonly ICartRepository cartRepository;

    public GetCartHandler(ICartRepository cartRepository)
    {
        this.cartRepository = cartRepository;
    }

    public Cart Handle(GetCartQuery query) =>
        this.cartRepository.Get(query.UserId);
}