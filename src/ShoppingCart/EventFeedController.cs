using Microsoft.AspNetCore.Mvc;

namespace ShoppingCart;

[ApiController]
[Route("events")]
public class EventFeedController : ControllerBase
{
    private readonly IEventStore eventStore;

    public EventFeedController(IEventStore eventStore)
    {
        this.eventStore = eventStore;
    }

    [HttpGet]
    public IActionResult GetEvents([FromQuery] long from = 0)
    {
        var events = this.eventStore.GetEventsFrom(from);
        return this.Ok(events);
    }
}
