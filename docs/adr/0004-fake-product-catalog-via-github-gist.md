# ADR-0004: Use a GitHub Gist as a fake Product Catalog endpoint

## Status
Accepted

## Date
2026-09-06

## Context
The Shopping Cart microservice depends on a Product Catalog microservice to
look up product information (see `IProductCatalogClient`). The book itself
never implements a real Product Catalog microservice in chapter 2 — instead,
it points `ProductCatalogClient` at a hardcoded JSON file hosted on GitHub
(a "git.io" short link to a static file), explicitly as a stand-in for a
real product catalog service.

We initially implemented `FakeProductCatalogClient` as an in-process class
returning hardcoded data with no network call at all. This let us build and
test the Application and API layers early, but it meant `ProductCatalogClient`
never exercised a real HTTP call — which is a core part of what the book
demonstrates in section 2.2.3–2.2.4, and a prerequisite for meaningfully
adding resilience (Polly) later.

We considered building a small stub ASP.NET service of our own to act as a
"real" Product Catalog microservice, but decided against it for now (see
Alternatives).

## Decision
Follow the book's own approach: host a static `products.json` file as a
GitHub Gist, and have `ProductCatalogClient` call it over HTTP using
`HttpClient`, registered via `AddHttpClient` (ASP.NET Core's recommended
`HttpClientFactory` pattern, replacing the book's simpler manual `HttpClient`
construction).

Because the Gist is a static file, it cannot filter by product ID via a
query string the way a real Product Catalog microservice would (see the
book's `?productIds=[1,2]` example). `ProductCatalogClient` therefore
fetches the entire product list on every call and filters it in memory
against the requested `productCatalogIds`.

## Consequences
**Positive**
- `ProductCatalogClient` now performs a genuine HTTP call to an external
  endpoint, matching the book's intent and giving us something real to
  later wrap in Polly resilience policies (chapter 7).
- No cost and minimal setup — a Gist is free and requires no hosting,
  deployment, or additional infrastructure to maintain.
- Easy to update product data during development by editing the Gist
  directly.

**Negative**
- The Gist cannot filter results server-side, so every call transfers the
  full product list regardless of how many items are actually requested.
  This is a meaningful deviation from how a real Product Catalog
  microservice would behave, and is only acceptable because the Gist is a
  temporary development stand-in, not a real dependency.
- The Gist has no authentication, versioning strategy, or uptime guarantee
  beyond GitHub's own gist hosting. Not suitable as a permanent dependency
  for anything beyond local development and coursework.
- Because we call the "always latest revision" raw URL, editing the Gist
  changes behavior for all running instances immediately, with no way to
  pin to a specific version without switching to the revision-specific URL.

## Alternatives considered
- **Keep the in-process `FakeProductCatalogClient`** — rejected, since it
  never exercises a real HTTP call and would make it artificial to later
  add HTTP-specific resilience patterns (retries, circuit breakers) around
  it.
- **Build a small stub Product Catalog microservice ourselves** (a minimal
  ASP.NET project with a hardcoded `/products` endpoint) — a more realistic
  option that would support proper query-string filtering, and one we may
  revisit later in the course. Rejected for now to avoid building and
  maintaining a throwaway service before the book itself introduces a real
  multi-service scenario (chapter 5, the Loyalty Program example).
- **Use a public JSON-placeholder-style mocking service** — rejected in
  favor of the Gist, since a Gist we control ourselves gives us the freedom
  to shape the data to match our own domain model exactly.