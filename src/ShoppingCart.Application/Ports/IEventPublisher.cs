namespace ShoppingCart.Application.Ports;

public interface IEventPublisher
{
    void Publish(string eventName, object content);
}