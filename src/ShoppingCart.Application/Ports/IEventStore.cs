namespace ShoppingCart.Application.Ports;

public interface IEventStore
{
    void Append(string eventName, object content);
    IEnumerable<Event> GetEventsFrom(long sequenceNumber);
}