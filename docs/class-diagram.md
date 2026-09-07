# Shopping Cart – Class Diagram

This diagram shows the classes, records, and interfaces of the Shopping Cart
microservice on this branch (`book-reference/flat-structure`), where everything
lives in a single flat project following the book's chapter 2 layout. See
`package-diagram.md` for the higher-level project view.

> For the Clean Architecture variant (Domain/Application/Infrastructure/API with
> use-case handlers and ports), see the `main` branch. This branch is the "before"
> (book) side of a before/after comparison.

```mermaid
classDiagram
    %% ===== Domain =====
    class ShoppingCart {
        -HashSet~ShoppingCartItem~ items
        +int UserId
        +IEnumerable~ShoppingCartItem~ Items
        +ShoppingCart(int userId)
        +AddItems(IEnumerable~ShoppingCartItem~ items, IEventStore eventStore)
        +RemoveItems(int[] productCatalogueIds, IEventStore eventStore)
    }

    class ShoppingCartItem {
        <<record>>
        +int ProductCatalogueId
        +string ProductName
        +string Description
        +Money Price
    }

    class Money {
        <<record>>
        +string Currency
        +decimal Amount
    }

    ShoppingCart "1" o-- "many" ShoppingCartItem : contains
    ShoppingCartItem --> Money : has
    ShoppingCart ..> IEventStore : raises events via

    %% ===== Store =====
    class IShoppingCartStore {
        <<interface>>
        +Get(int userId) ShoppingCart
        +Save(ShoppingCart shoppingCart)
    }

    class InMemoryShoppingCartStore {
        -ConcurrentDictionary~int,ShoppingCart~ carts
        +Get(int userId) ShoppingCart
        +Save(ShoppingCart shoppingCart)
    }

    InMemoryShoppingCartStore ..|> IShoppingCartStore

    %% ===== Events =====
    class IEventStore {
        <<interface>>
        +Append(string eventName, object content)
        +GetEventsFrom(long sequenceNumber) IEnumerable~Event~
    }

    class EventStore {
        -ConcurrentQueue~Event~ events
        -long nextSequenceNumber
        +Append(string eventName, object content)
        +GetEventsFrom(long sequenceNumber) IEnumerable~Event~
    }

    class Event {
        <<record>>
        +long SequenceNumber
        +string EventName
        +object Content
    }

    EventStore ..|> IEventStore
    IEventStore ..> Event : produces

    %% ===== Product catalog =====
    class ProductCatalogClient {
        -HttpClient client
        +GetCartItems(int[] productCatalogIds) Task~IEnumerable~ShoppingCartItem~~
    }

    ProductCatalogClient ..> ShoppingCartItem : builds

    %% ===== Web =====
    class ShoppingCartController {
        -IShoppingCartStore shoppingCartStore
        -ProductCatalogClient productCatalogClient
        -IEventStore eventStore
        +Get(int userId) IActionResult
        +AddItems(int userId, int[] productIds) Task~IActionResult~
        +RemoveItems(int userId, int[] productIds) IActionResult
        +GetEvents(long from) IActionResult
    }

    ShoppingCartController --> IShoppingCartStore
    ShoppingCartController --> ProductCatalogClient
    ShoppingCartController --> IEventStore
    ShoppingCartController ..> ShoppingCart
```

## Notes

- **Records** (`ShoppingCartItem`, `Money`, `Event`) use the `<<record>>` stereotype — immutable, data-carrying types.
- **The domain object raises its own events.** `ShoppingCart.AddItems`/`RemoveItems` receive an `IEventStore` and append `CartItemAdded`/`CartItemRemoved` events directly. There is no separate Application layer between the controller and the domain (contrast with the Clean Architecture branch, ADR-0002).
- **The controller does all orchestration inline** — it depends directly on the store, the product catalog client, and the event store, with no use-case/handler classes in between.
- **`ProductCatalogClient` has no interface** — the controller depends on the concrete class. Only `IShoppingCartStore` and `IEventStore` are kept as thin interfaces, matching the book, so DI registration reads naturally.
- **`ShoppingCart` and its namespace share the name `ShoppingCart`** (the project's root namespace) — this is exactly how the book structures it; C# resolves the type within the namespace without conflict.
- Multiplicity `ShoppingCart "1" o-- "many" ShoppingCartItem` reflects that a cart aggregates zero or more items (backed by a `HashSet<ShoppingCartItem>` in the implementation).
