# ADR-0001: Adopt Clean Architecture layering instead of the book's flat structure

## Status
Accepted

## Date
2026-09-05

## Context
"Microservices in .NET" (2nd ed.) implements each microservice as a single
ASP.NET project, mixing HTTP controllers, domain logic, and infrastructure
concerns (HTTP clients, data access) in one project (see chapter 2, the
Shopping Cart microservice).

We have prior experience with Clean Architecture and believe its layering
principles combine well with microservice design — each service can still be
a small, independently deployable unit, while internally separating concerns
for testability and maintainability.

## Decision
Each microservice will be split into four projects:

- **Domain** — entities, value objects, and domain logic. No dependencies on
  any other project or framework.
- **Application** — use cases and ports (interfaces) that define what the
  application needs from the outside world (repositories, external clients,
  event publishing). Depends only on Domain.
- **Infrastructure** — concrete implementations of the Application ports
  (HTTP clients, in-memory or database repositories, event stores). Depends
  on Application.
- **API** — ASP.NET controllers and startup/DI wiring. Depends on
  Application and Infrastructure.

Dependency direction: `API → Application → Domain`, and
`Infrastructure → Application → Domain`. Domain has zero outward
dependencies.

## Consequences
**Positive**
- Domain and Application logic can be unit tested without a running web
  server, a database, or HTTP mocking.
- Business rules are decoupled from framework and infrastructure choices,
  making it easier to swap technologies later.
- Matches how many production .NET microservices are structured in practice,
  which is valuable experience for the diploma project.

**Negative**
- More projects and files than the book's single-project examples, adding
  some ceremony for the small examples in early chapters.
- Requires translating each of the book's code listings into the appropriate
  layer, rather than copying them verbatim.

## Alternatives considered
- **Follow the book's flat structure verbatim** — rejected because it mixes
  concerns and makes unit testing domain/application logic harder without
  spinning up ASP.NET infrastructure.