# Shopping Cart Microservice

Part of the **Diplom i Softwareudvikling** module *Udvikling af store systemer*, built while working through **"Microservices in .NET" (2nd Edition)** by Christian Horsdal Gammelgaard.

This project implements the book's Shopping Cart microservice example (chapter 2), adapted from .NET 5 to **.NET 10**.

> **Branch note.** This is the `book-reference/flat-structure` branch, which follows the book's **single, flat project** layout so it can serve as the "before" side of a before/after comparison in the report. The `main` branch restructures the same service with **Clean Architecture** layering; see [`docs/adr`](docs/adr) for the reasoning behind those deviations from the book.

## Architecture

The service is a **single ASP.NET Core project** (`src/ShoppingCart`) with all source files in one place, exactly as the book builds it in chapter 2. There is no Domain/Application/Infrastructure/API split:

- **Domain types** — `ShoppingCart`, `ShoppingCartItem`, `Money`. The `ShoppingCart` domain object raises its own events (`AddItems`/`RemoveItems` take an `IEventStore` and append events directly).
- **`ShoppingCartController`** — does all orchestration inline (get cart, call product catalog, save). No separate use-case/handler classes.
- **`EventFeedController`** — exposes the readable event feed (`GET /events`) that other microservices poll, kept separate from the cart controller so each controller has a single responsibility.
- **`ProductCatalogClient`** — an HTTP client used as a concrete class, with no port interface around it.
- **`IShoppingCartStore` / `InMemoryShoppingCartStore`** and **`IEventStore` / `EventStore`** — thin storage interfaces plus in-memory implementations. The interfaces are kept only so DI registration reads naturally, as the book itself does.
- **`Program.cs`** — dependency injection wiring for the concrete classes.

See [`docs/package-diagram.md`](docs/package-diagram.md) for a diagram of the project structure, and [`docs/class-diagram.md`](docs/class-diagram.md) for the detailed class relationships.

The ADRs in [`docs/adr`](docs/adr) document the design decisions made on the **`main`** (Clean Architecture) branch. They are kept on this branch as-is because they are precisely the "after" that this flat structure is being compared against:

| ADR | Decision (applies to the `main` branch) |
|---|---|
| [0001](docs/adr/0001-clean-architecture-layering.md) | Adopt Clean Architecture layering instead of the book's flat structure |
| [0002](docs/adr/0002-event-publishing-in-application-layer.md) | Raise domain events from the Application layer, not the Domain layer |
| [0003](docs/adr/0003-event-store-with-readable-feed.md) | Implement events as a readable feed (`IEventStore`) instead of fire-and-forget publishing |
| [0004](docs/adr/0004-fake-product-catalog-via-github-gist.md) | Use a GitHub Gist as a fake Product Catalog endpoint |

## Running the service

**Prerequisites:** .NET 10 SDK

```powershell
dotnet run --project src\ShoppingCart
```

The API will start on a local port shown in the console output (e.g. `http://localhost:5087`).

### Endpoints

| Method | Route | Description |
|---|---|---|
| `GET` | `/cart/{userId}` | Get a user's cart (creates an empty one if none exists) |
| `POST` | `/cart/{userId}/items` | Add items to a user's cart. Body: array of product IDs, e.g. `[1, 2, 3]` |
| `DELETE` | `/cart/{userId}/items` | Remove items from a user's cart. Body: array of product IDs |
| `GET` | `/events?from={sequenceNumber}` | Read published events from a given sequence number onward |

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
└── src/
    └── ShoppingCart/
        ├── ShoppingCart.csproj
        ├── Program.cs
        ├── ShoppingCart.cs              (domain object; raises its own events)
        ├── ShoppingCartItem.cs
        ├── Money.cs
        ├── ShoppingCartController.cs    (cart orchestration inline)
        ├── EventFeedController.cs       (readable event feed endpoint)
        ├── ProductCatalogClient.cs      (concrete, no interface)
        ├── IShoppingCartStore.cs
        ├── InMemoryShoppingCartStore.cs
        ├── IEventStore.cs
        ├── EventStore.cs
        └── Event.cs
```

## Known limitations (by design, for now)

- **No persistence** — `InMemoryShoppingCartStore` and `EventStore` lose all data on restart. Acceptable for this stage of the course; revisited when the book covers data storage (chapter 6).
- **No resilience policies** — HTTP calls to the Product Catalog gist have no retry, timeout, or circuit breaker yet. Deliberately deferred to align with the book's own introduction of Polly in chapter 7.
- **No authentication/authorization** — not yet covered by the book at this stage (chapter 10).

## AI transparency

The refactoring from the Clean Architecture layout (`main`) to this flat, book-faithful structure on the `book-reference/flat-structure` branch — including the code restructuring and the updates to this README and the diagrams in [`docs/`](docs) — was carried out with **Claude Code (model Opus 4.8)**. This note is included in the interest of openness about the use of AI in the project.
