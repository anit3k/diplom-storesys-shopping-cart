# ADR-0003: Implement events as a readable feed (IEventStore) instead of fire-and-forget publishing

## Status
Accepted

## Date
2026-09-06

## Context
The book (chapter 2, section 2.2.6) implements a simple event feed: the
domain model raises events when items are added to or removed from a cart,
and an `EventFeed` component allows other microservices to read published
events. This lets other services (e.g., recommendations, shopper tracking)
poll for changes rather than requiring a direct call from the Shopping Cart
microservice to each subscriber.

Our first implementation (see ADR-0002) introduced an `IEventPublisher` port
with a single `Publish(eventName, content)` method, backed by a
`ConsoleEventPublisher` that only wrote events to the console. This covered
"announcing that something happened" but did not allow any other service —
or even our own API — to retrieve events after the fact. It was fire-and-
forget, not a feed.

This falls short of the book's intent: the whole point of an event feed in a
microservice architecture is that consumers can catch up on events they
missed, in order, rather than only reacting to events in real time.

## Decision
Replace `IEventPublisher` with `IEventStore`, which both records events and
allows them to be read back by sequence number:

```csharp
public interface IEventStore
{
    void Append(string eventName, object content);
    IEnumerable<Event> GetEventsFrom(long sequenceNumber);
}
```

Each `Event` carries a monotonically increasing `SequenceNumber`, so
consumers can track how far they've read and request only newer events on
their next poll (`GetEventsFrom(lastSeenSequenceNumber + 1)`).

The concrete implementation, `InMemoryEventStore` (Infrastructure layer),
stores events in a thread-safe in-memory collection and assigns sequence
numbers using `Interlocked.Increment` to remain correct under concurrent
requests.

A new use case, `GetEventsHandler`, exposes this through a
`GET /cart/events?from={sequenceNumber}` endpoint, so other microservices
can poll the Shopping Cart API directly — matching the book's intent of an
HTTP-based event feed.

## Consequences
**Positive**
- Other services can catch up on missed events after downtime or restarts,
  rather than only receiving events they happened to be listening for at
  the moment they occurred.
- Matches the book's architectural intent (a pollable event feed) more
  closely than our initial fire-and-forget publisher.
- The sequence-number-based read model is a stepping stone toward the more
  advanced event-feed patterns the book introduces in chapter 5.

**Negative**
- Events are stored only in memory; they are lost on service restart. This
  is acceptable for chapter 2's scope but will need to be revisited (e.g.,
  a persistent store) before this could be considered production-ready.
- The in-memory store grows unbounded, since events are never removed. Fine
  for local development and coursework; not suitable as-is for a long-running
  production service.
- `IEventStore` combines two responsibilities (writing and reading events).
  This is a deliberate simplification for chapter 2 — a later iteration
  could split this into separate write/read ports (command-query
  separation) if that proves valuable.

## Alternatives considered
- **Keep the fire-and-forget `IEventPublisher`** — rejected, as it does not
  satisfy the book's requirement that other services can read the event
  feed independently of when events were originally raised.
- **Use a real message broker (e.g., RabbitMQ, Azure Service Bus) already at
  this stage** — considered, but rejected for now as premature: the book
  introduces more advanced event-feed and messaging patterns in chapter 5,
  and introducing a broker here would jump ahead of the book's own pacing
  and add infrastructure complexity not yet justified by the current scope.