# ADR-0002: Raise domain events from the Application layer, not the Domain layer

## Status
Accepted

## Date
2026-09-05

## Context
In the book (chapter 2, listing 2.15), the `ShoppingCart` domain class raises
events directly by taking an `IEventStore` as a parameter to `AddItems` and
`RemoveItems`, and calling `eventStore.Raise(...)` when items change.

Per ADR-0001, our Domain layer must have zero outward dependencies —
including dependencies on ports/interfaces defined in the Application layer.
Accepting an `IEventStore`/`IEventPublisher` parameter in a Domain method
would violate that boundary.

## Decision
The Domain layer (`Cart`, `CartItem`, `Money`) contains no knowledge of
events. Event publishing is orchestrated by the Application layer's use case
handlers (e.g., `AddItemsToCartHandler`), which call an `IEventPublisher`
port after invoking domain behavior (`cart.AddItems(...)`) and persisting the
result.

## Consequences
**Positive**
- Domain stays fully independent and trivially unit-testable with no mocks
  required.
- Use case handlers become the single place where "what happens after a
  domain change" (persistence, events) is coordinated — easier to read end
  to end.

**Negative**
- Domain objects cannot guarantee that an event is *always* raised whenever
  a state change occurs, since that responsibility now lives outside the
  entity. This relies on discipline in the Application layer (or, later, a
  domain-events collection pattern if we want to bring that guarantee back
  into the Domain layer without violating dependency direction).

## Alternatives considered
- **Follow the book exactly** (pass `IEventStore` into domain methods) —
  rejected per ADR-0001's dependency rule.
- **Domain event collection pattern** (entities collect events internally in
  a list, without depending on any interface; Application layer reads and
  dispatches them after the fact) — a valid alternative that preserves the
  "always raised" guarantee. Not chosen now for simplicity, but noted here as
  a possible future refinement if event consistency becomes a concern.