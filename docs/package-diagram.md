# Shopping Cart – Package Diagram

This diagram shows the current project structure of the Shopping Cart microservice, following Clean Architecture layering (see ADR-0001). Arrows indicate project references (dependency direction).

```mermaid
graph TD
    API["ShoppingCart.API<br/><i>Controllers, Program.cs, DI wiring</i>"]
    Application["ShoppingCart.Application<br/><i>Use cases, Ports (interfaces)</i>"]
    Domain["ShoppingCart.Domain<br/><i>Cart, CartItem, Money</i>"]
    Infrastructure["ShoppingCart.Infrastructure<br/><i>InMemoryCartRepository, ProductCatalogClient,<br/>InMemoryEventStore</i>"]

    API --> Application
    API --> Infrastructure
    Infrastructure --> Application
    Application --> Domain

    DomainTests["ShoppingCart.Domain.Tests"]
    ApplicationTests["ShoppingCart.Application.Tests"]

    DomainTests -.-> Domain
    ApplicationTests -.-> Application

    ExternalGist(("Gist:<br/>products.json<br/>(fake Product Catalog)"))
    Infrastructure -.HTTP GET.-> ExternalGist

    style Domain fill:#2d5a3d,stroke:#4ade80,color:#fff
    style Application fill:#2d4a5a,stroke:#60a5fa,color:#fff
    style Infrastructure fill:#5a4a2d,stroke:#fbbf24,color:#fff
    style API fill:#4a2d5a,stroke:#c084fc,color:#fff
    style ExternalGist fill:#333,stroke:#999,color:#fff
```

## Notes

- **Domain** has zero outward dependencies — no reference to any other project (ADR-0001).
- **Application** depends only on **Domain**. It defines *ports* (`ICartRepository`, `IProductCatalogClient`, `IEventStore`) that describe what it needs from the outside world, without knowing how those needs are fulfilled (Dependency Inversion Principle).
- **Infrastructure** depends on **Application**, implementing its ports with concrete technology choices (in-memory storage, HTTP client). It does not depend on **API**.
- **API** depends on both **Application** (to call use case handlers) and **Infrastructure** (only in `Program.cs`, for dependency injection wiring). Controllers only ever call into **Application**.
- **Infrastructure** makes an outbound HTTP call to a GitHub Gist acting as a fake Product Catalog microservice (ADR-0004).
- Test projects reference only the layer they test, keeping tests isolated from framework and infrastructure concerns.
