# Shopping Cart Microservice

Part of the **Diplom i Softwareudvikling** module *Udvikling af store systemer*, built while working through **"Microservices in .NET" (2nd Edition)** by Christian Horsdal Gammelgaard.

This project implements the book's Shopping Cart microservice example (chapter 2), adapted from .NET 5 to **.NET 10**, and restructured with **Clean Architecture** layering instead of the book's single-project approach. See [`docs/adr`](docs/adr) for the reasoning behind the deviations from the book.

## Architecture

The service is split into four projects, following Clean Architecture's dependency rule (dependencies point inward, toward the Domain):

- **`ShoppingCart.Domain`** — Entities and value objects (`Cart`, `CartItem`, `Money`). No dependencies on any other project.
- **`ShoppingCart.Application`** — Use cases (`AddItemsToCart`, `GetCart`, `RemoveItemsFromCart`, `GetEvents`) and ports (interfaces) describing what the application needs from the outside world.
- **`ShoppingCart.Infrastructure`** — Implementations of the Application ports: an in-memory cart repository, an HTTP-based product catalog client, and an in-memory event store.
- **`ShoppingCart.API`** — ASP.NET Core controllers and dependency injection wiring.

See [`docs/package-diagram.md`](docs/package-diagram.md) for a diagram of the project structure, and [`docs/class-diagram.md`](docs/class-diagram.md) for the detailed class relationships.

Architecture decisions are documented as ADRs in [`docs/adr`](docs/adr):

| ADR | Decision |
|---|---|
| [0001](docs/adr/0001-clean-architecture-layering.md) | Adopt Clean Architecture layering instead of the book's flat structure |
| [0002](docs/adr/0002-event-publishing-in-application-layer.md) | Raise domain events from the Application layer, not the Domain layer |
| [0003](docs/adr/0003-event-store-with-readable-feed.md) | Implement events as a readable feed (`IEventStore`) instead of fire-and-forget publishing |
| [0004](docs/adr/0004-fake-product-catalog-via-github-gist.md) | Use a GitHub Gist as a fake Product Catalog endpoint |

## Running the service

**Prerequisites:** .NET 10 SDK

```powershell
dotnet run --project src\ShoppingCart.API
```

The API will start on a local port shown in the console output (e.g. `http://localhost:5087`).

### Endpoints

| Method | Route | Description |
|---|---|---|
| `GET` | `/cart/{userId}` | Get a user's cart (creates an empty one if none exists) |
| `POST` | `/cart/{userId}/items` | Add items to a user's cart. Body: array of product IDs, e.g. `[1, 2, 3]` |
| `DELETE` | `/cart/{userId}/items` | Remove items from a user's cart. Body: array of product IDs |
| `GET` | `/cart/events?from={sequenceNumber}` | Read published events from a given sequence number onward |

## Testing the API

A Postman collection is included: [`ShoppingCart.postman_collection.json`](ShoppingCart.postman_collection.json).

1. Import it into Postman.
2. Update the `baseUrl` collection variable to match the port shown when you run the API.
3. Run the requests in order: **Get Cart → Add Items To Cart → Get Cart → Remove Items From Cart → Get Events**.

## Product data

Product information is fetched from a static [GitHub Gist](https://gist.github.com/anit3k/cf11fd86dce483e3963f13d5d30122ae) acting as a fake Product Catalog microservice, following the same approach the book itself uses (see ADR-0004). This is a temporary development stand-in — it always returns the full product list, since a static file cannot filter server-side by product ID the way a real microservice endpoint would.

## Solution structure

```
shopping-cart/
├── ShoppingCart.slnx
├── ShoppingCart.postman_collection.json
├── docs/
│   ├── adr/
│   │   ├── template.md
│   │   ├── 0001-clean-architecture-layering.md
│   │   ├── 0002-event-publishing-in-application-layer.md
│   │   ├── 0003-event-store-with-readable-feed.md
│   │   └── 0004-fake-product-catalog-via-github-gist.md
│   ├── package-diagram.md
│   └── class-diagram.md
├── src/
│   ├── ShoppingCart.Domain/
│   ├── ShoppingCart.Application/
│   ├── ShoppingCart.Infrastructure/
│   └── ShoppingCart.API/
└── tests/
    ├── ShoppingCart.Domain.Tests/
    └── ShoppingCart.Application.Tests/
```

## Known limitations (by design, for now)

- **No persistence** — `InMemoryCartRepository` and `InMemoryEventStore` lose all data on restart. Acceptable for this stage of the course; revisited when the book covers data storage (chapter 6).
- **No resilience policies** — HTTP calls to the Product Catalog gist have no retry, timeout, or circuit breaker yet. Deliberately deferred to align with the book's own introduction of Polly in chapter 7.
- **No authentication/authorization** — not yet covered by the book at this stage (chapter 10).
