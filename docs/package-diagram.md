# Shopping Cart – Package Diagram

This diagram shows the project structure of the Shopping Cart microservice on this
branch (`book-reference/flat-structure`): a **single, flat ASP.NET Core project**
that mirrors how the book (*Microservices in .NET*, 2nd ed., chapter 2) builds the
service. There is no separate Domain/Application/Infrastructure/API split — all
source files live directly in one project.

> For the Clean Architecture variant (four projects), see the `main` branch and
> its ADRs. This branch exists as the "before" (book) side of a before/after
> comparison.

```mermaid
graph TD
    subgraph Project["ShoppingCart (single ASP.NET Core project)"]
        Controller["ShoppingCartController<br/><i>all orchestration inline</i>"]
        Domain["ShoppingCart, ShoppingCartItem, Money<br/><i>domain types; events raised from AddItems/RemoveItems</i>"]
        Store["IShoppingCartStore / InMemoryShoppingCartStore"]
        Events["IEventStore / EventStore, Event"]
        Catalog["ProductCatalogClient<br/><i>concrete class, no interface</i>"]
        Program["Program.cs<br/><i>DI wiring</i>"]

        Controller --> Store
        Controller --> Catalog
        Controller --> Domain
        Controller --> Events
        Domain --> Events
        Catalog --> Domain
        Store --> Domain
    end

    ExternalGist(("Gist:<br/>products.json<br/>(fake Product Catalog)"))
    Catalog -.HTTP GET.-> ExternalGist

    style Project fill:#1f2937,stroke:#94a3b8,color:#fff
    style Domain fill:#2d5a3d,stroke:#4ade80,color:#fff
    style Store fill:#2d4a5a,stroke:#60a5fa,color:#fff
    style Events fill:#2d4a5a,stroke:#60a5fa,color:#fff
    style Catalog fill:#5a4a2d,stroke:#fbbf24,color:#fff
    style Controller fill:#4a2d5a,stroke:#c084fc,color:#fff
    style Program fill:#4a2d5a,stroke:#c084fc,color:#fff
    style ExternalGist fill:#333,stroke:#999,color:#fff
```

## Notes

- **One project, no layer boundaries.** Everything compiles into a single assembly. Types reference each other directly, without project references or a dependency-inversion boundary between "application" and "infrastructure".
- **The controller orchestrates directly.** `ShoppingCartController` fetches the cart, calls `ProductCatalogClient`, mutates the domain object, saves it, and reads events — there are no separate use-case/handler classes.
- **The domain raises its own events.** `ShoppingCart.AddItems`/`RemoveItems` take an `IEventStore` and append events themselves, rather than an Application layer doing it (contrast with ADR-0002 on `main`).
- **`ProductCatalogClient` is used as a concrete class** — no `IProductCatalogClient` port. `IShoppingCartStore` and `IEventStore` are kept only as thin interfaces so the DI registration reads naturally, as the book itself does.
- **No test projects** on this branch — the book's chapter 2 layout is a single project, and the two (empty) test projects were removed here to keep the flat structure faithful. The Clean Architecture branch keeps them.
- The service still makes an outbound HTTP call to a GitHub Gist acting as a fake Product Catalog microservice (ADR-0004).
