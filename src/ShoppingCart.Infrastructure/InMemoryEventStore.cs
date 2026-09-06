using System.Collections.Concurrent;
using ShoppingCart.Application.Ports;

namespace ShoppingCart.Infrastructure;

public class InMemoryEventStore : IEventStore
{
    private readonly ConcurrentQueue<Event> events = new();
    private long nextSequenceNumber = 1;

    public void Append(string eventName, object content)
    {
        var sequenceNumber = Interlocked.Increment(ref this.nextSequenceNumber) - 1;
        this.events.Enqueue(new Event(sequenceNumber, eventName, content));
    }

    public IEnumerable<Event> GetEventsFrom(long sequenceNumber) =>
        this.events.Where(e => e.SequenceNumber >= sequenceNumber).ToList();
}