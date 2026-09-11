using Microsoft.AspNetCore.Mvc;
using ShoppingCart.Application.UseCases.GetEvents;

namespace ShoppingCart.API.Controllers;

[ApiController]
[Route("events")]
public class EventsController : ControllerBase
{
    private readonly GetEventsHandler getEventsHandler;

    public EventsController(GetEventsHandler getEventsHandler)
    {
        this.getEventsHandler = getEventsHandler;
    }

    [HttpGet]
    public IActionResult GetEvents([FromQuery] long from = 0)
    {
        var events = this.getEventsHandler.Handle(new GetEventsQuery(from));
        return this.Ok(events);
    }
}
