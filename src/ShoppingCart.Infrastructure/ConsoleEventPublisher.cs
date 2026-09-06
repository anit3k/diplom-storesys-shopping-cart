using ShoppingCart.Application.Ports;

namespace ShoppingCart.Infrastructure;

public class ConsoleEventPublisher : IEventPublisher
{
    public void Publish(string eventName, object content) =>
        Console.WriteLine($"[Event] {eventName}: {System.Text.Json.JsonSerializer.Serialize(content)}");
}