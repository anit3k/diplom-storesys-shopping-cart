namespace ShoppingCart;

public interface IEventStore
{
    void Append(string eventName, object content);
    IEnumerable<Event> GetEventsFrom(long sequenceNumber);
}
