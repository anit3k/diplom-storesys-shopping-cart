using ShoppingCart.Application.Ports;

namespace ShoppingCart.Application.UseCases.GetEvents;

public class GetEventsHandler
{
    private readonly IEventStore eventStore;

    public GetEventsHandler(IEventStore eventStore)
    {
        this.eventStore = eventStore;
    }

    public IEnumerable<Event> Handle(GetEventsQuery query) =>
        this.eventStore.GetEventsFrom(query.FromSequenceNumber);
}