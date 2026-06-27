# WebAPI Shop

A scalable e‑commerce REST API built with **ASP.NET Core / .NET 9**, designed around a clean layered architecture and extended with production‑oriented concerns: distributed caching, event‑driven messaging, authentication, and resilience.

## Overview

WebAPI Shop started as a classic three‑layer Web API and grew into a small **distributed system**. Beyond the core CRUD API it includes a Redis cache layer, a Kafka‑based event pipeline with a dedicated consumer microservice, JWT authentication with role‑based authorization, server‑side rate limiting, and a client with automatic retry/backoff. Everything is containerized via Docker Compose.

This README focuses on the **architecture and structure** of the system rather than on individual endpoints.

## Technology Stack

| Concern              | Technology                                             |
| -------------------- | ------------------------------------------------------ |
| Framework / Language | ASP.NET Core (.NET 9), C#                              |
| Data Access          | Entity Framework Core (SQL Server)                     |
| Mapping              | AutoMapper                                             |
| Caching              | Redis (StackExchange.Redis)                            |
| Messaging            | Apache Kafka (Confluent.Kafka)                         |
| Authentication       | JWT Bearer (HttpOnly cookie) + role‑based authorization|
| Password security    | BCrypt hashing + Zxcvbn strength scoring               |
| Rate limiting        | ASP.NET Core sliding‑window rate limiter               |
| Logging              | NLog                                                   |
| Testing              | xUnit, Moq, Moq.EntityFrameworkCore                    |
| Containerization     | Docker / Docker Compose                                |

## Solution Architecture

The solution is a **multi‑project .NET solution** with a strict dependency direction. Higher layers depend on lower ones; the inner layers (Entities, DTOs) have no knowledge of the web host.

```
                        ┌─────────────────────────────┐
   HTTP / JSON  ───────▶│  WebApiShop (API host)      │  Controllers, Middleware,
                        │                             │  Auth, Redis cache-aside,
                        │                             │  static frontend (wwwroot)
                        └──────────────┬──────────────┘
                                       │
                        ┌──────────────▼──────────────┐
                        │  Services (Business Logic)  │  AutoMapper, validation,
                        │                             │  JWT/token, Kafka producer
                        └──────────────┬──────────────┘
                                       │
                        ┌──────────────▼──────────────┐
                        │  Repository (Data Access)   │  EF Core, DbContext,
                        │                             │  Migrations
                        └──────────────┬──────────────┘
                                       │
              ┌────────────────────────┴────────────────────────┐
              │                                                  │
        ┌─────▼──────┐                                    ┌──────▼──────┐
        │  Entities  │  (EF domain models)                │    DTOs     │ (API contracts, records)
        └────────────┘                                    └─────────────┘
```

### Projects

| Project                    | Responsibility                                                                                  |
| -------------------------- | ----------------------------------------------------------------------------------------------- |
| **WebApiShop**             | The API host: controllers, middleware pipeline, authentication, Redis cache‑aside, Swagger, and the static frontend in `wwwroot`. |
| **Services**               | Business logic, DTO ↔ entity mapping (AutoMapper), JWT token creation, password handling, and the Kafka producer. |
| **Repository**             | Data access with EF Core — `WebApiShopContext`, repository interfaces/implementations, and migrations. |
| **Entities**               | EF Core domain models (`Product`, `Order`, `OrderItem`, `Category`, `User`, `Rating`, `Password`). |
| **DTOs**                   | Data Transfer Objects implemented as C# `record`s — the public API contract, decoupled from the domain models. |
| **Tests**                  | Unit and integration tests (xUnit + Moq, EF Core integration via a DB fixture).                 |
| **BillingServiceConsumer** | A **standalone microservice** (console app) that consumes order events from Kafka. Independently deployable. |

### Design principles

- **Separation of concerns / dependency inversion** — layers communicate through interfaces (`IUsersService`, `IProductsRepository`, …) wired up with built‑in DI in `Program.cs`.
- **DTO boundary** — DTOs (records) are the only types crossing the API boundary, preventing leakage of EF entities and avoiding circular references.
- **Async all the way down** — every data‑access and I/O path is `async`/`await` for thread efficiency under load.

## Cross‑Cutting Architecture

### Caching — Redis cache‑aside

`ProductsController` implements the **cache‑aside pattern** against Redis:

- Read endpoints build a cache key from the request path + query string, return the cached value on a hit, and otherwise query the service and populate the cache with a configurable TTL (`Redis:DefaultTTLInMinutes`).
- Write endpoints (`POST`/`PUT`, admin‑only) **invalidate** product cache entries, using a `SCAN`‑based key sweep (`/api/Products*`) so the cache is never left stale.
- Cache access is wrapped defensively — if Redis is unavailable the controller **falls back to the database** rather than failing the request.

### Messaging — Kafka event‑driven pipeline

Order creation is **event‑driven and decoupled**:

1. When an order is created, `OrdersService` publishes the order (serialized to JSON, keyed by order id) to the Kafka `orders` topic via `KafkaProducerService` (registered as a singleton).
2. Publishing is **fire‑and‑forget with isolation**: a messaging failure is logged but never breaks order creation — the write path stays available even if Kafka is down.
3. **`BillingServiceConsumer`** is a separate process that subscribes to the `orders` topic (consumer group `billing-service`) and processes events independently — the seed of a microservices/billing pipeline.

This gives a loosely‑coupled, scalable design where downstream consumers can be added without touching the API.

### Security — Authentication & Authorization

- **JWT Bearer** authentication. Tokens are issued on registration/login (`TokenService`) and delivered in an **HttpOnly, Secure, SameSite=Strict cookie**; a `JwtBearerEvents.OnMessageReceived` hook extracts the token from the cookie so the browser never handles it directly.
- **Role‑based authorization** via a custom `[AuthorizeRole(Roles.Admin, …)]` attribute. Roles (`Admin`, `User`) are carried as claims; e.g. product mutations and the full user list are admin‑only.
- **Passwords** are hashed with **BCrypt** (never stored in plain text) and strength‑scored with **Zxcvbn** before acceptance.

### Resilience — Rate limiting & client retry

- **Server‑side**: a **sliding‑window rate limiter** partitioned per client IP (100 requests / minute, 6 segments) returns `429 Too Many Requests` when exceeded.
- **Client‑side**: the browser frontend wraps `fetch` in a `fetchWithRetry` helper that retries network errors, `429`, and `5xx` responses using **exponential backoff**, honoring the `Retry-After` header — a matched pair of throttling + back‑pressure handling.

### Request pipeline (middleware order)

The HTTP pipeline in `Program.cs` is ordered deliberately:

```
HTTPS redirect → Rate limiter → Rating middleware → Error handling → Static files → Authentication → Authorization → Controllers
```

- **Rating middleware** records every request into the rating table for traffic monitoring / analytics.
- **Error‑handling middleware** centralizes exception handling: exceptions are logged (NLog) and translated into appropriate HTTP responses.

## Infrastructure & Local Development

### Docker Compose

`docker-compose.yml` provisions the supporting infrastructure on a shared bridge network:

| Service     | Purpose                                              |
| ----------- | ---------------------------------------------------- |
| `cache-db`  | Redis (password‑protected) for the cache layer       |
| `kafka`     | Apache Kafka in KRaft mode (no ZooKeeper)            |
| `kafka-ui`  | Web UI for inspecting topics/messages (port `8080`)  |

The API itself ships with a **multi‑stage `Dockerfile`** (SDK build → publish → slim ASP.NET runtime) that restores each layer's `.csproj` separately for efficient layer caching.

### Configuration

Configuration lives in `appsettings.json` (environment overrides supported). Key sections:

```json
{
  "Jwt":   { "Key": "...", "Issuer": "WebApiShop", "Audience": "WebApiShopClient", "ExpireMinutes": 60 },
  "Kafka": { "BootstrapServers": "localhost:9092", "Topic": "orders" },
  "Redis": { "ConnectionString": "...", "DefaultTTLInMinutes": 30 },
  "ConnectionStrings": { "Home": "Data Source=...;Initial Catalog=WebApiShop;..." }
}
```

> The committed `Jwt:Key` and Redis password are development placeholders — replace them with secrets in any real deployment.

## Getting Started

### Prerequisites

- .NET 9 SDK
- Docker Desktop (for Redis + Kafka)
- SQL Server (or SQL Server Express)

### Run

```bash
# 1. Clone
git clone https://github.com/yael184/WebApiShop.git
cd WebApiShop

# 2. Start infrastructure (Redis, Kafka, Kafka UI)
docker compose up -d

# 3. Apply EF Core migrations / ensure the database exists
#    (connection string: ConnectionStrings:Home)

# 4. Run the API
dotnet run --project WebApiShop

# 5. (Optional) Run the billing consumer microservice
dotnet run --project BillingServiceConsumer

# 6. Run the tests
dotnet test
```

In Development, Swagger UI is available for exploring and authenticating against the API.

## Testing

- **Unit tests** isolate business logic with **Moq** mocking the repository layer.
- **Integration tests** exercise the EF Core repositories against a real database via a shared `DataBaseFixture`.

## Project Structure

```
WebApiShop/
├── WebApiShop/                 # API host (Controllers, Middleware, Authorization, wwwroot)
│   ├── Controllers/
│   ├── MiddleWare/             # Rating + error-handling middleware
│   ├── Authorization/          # AuthorizeRole attribute + Roles
│   └── wwwroot/                # Static frontend (fetch-with-retry client)
├── Services/                   # Business logic, AutoMapper, JWT, Kafka producer
├── Repository/                 # EF Core DbContext, repositories, Migrations
├── Entities/                   # EF Core domain models
├── DTOs/                       # API contracts (records)
├── Tests/                      # xUnit unit + integration tests
├── BillingServiceConsumer/     # Standalone Kafka consumer microservice
├── docker-compose.yml          # Redis, Kafka, Kafka UI
└── Dockerfile                  # Multi-stage build for the API
```
