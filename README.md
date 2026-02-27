# Bazario API

A production-grade E-Commerce REST API built as a deliberate learning project. Every architectural decision is intentional and documented — this is not a tutorial clone, it is a reference implementation.

Built with **.NET 9**, **ASP.NET Core**, **Entity Framework Core 9**, and **SQL Server**.

---

## What This Project Demonstrates

| Concern | Implementation |
|---|---|
| Architecture | Onion Architecture (4 layers, strict dependency rules) |
| Domain modeling | Domain-Driven Design — aggregates, value objects, domain events |
| Command/Query separation | Manual CQRS (no MediatR — patterns without magic) |
| Data access | EF Core 9 with Fluent API, Repository Pattern, Unit of Work |
| Error handling | Result Pattern — no exceptions for control flow |
| Authentication | JWT with refresh token rotation, role-based authorization |
| Testing | xUnit + Moq + FluentAssertions — domain unit tests and handler tests |
| Database | SQL Server in Docker, EF Core migrations |
| API design | REST conventions, proper HTTP semantics, Swagger/OpenAPI |

---

## Architecture

```
┌─────────────────────────────────────────────────────┐
│                    API Layer                        │  ← Controllers, Middleware, DI wiring
├─────────────────────────────────────────────────────┤
│               Infrastructure Layer                  │  ← EF Core, Repositories, JWT, Email
├─────────────────────────────────────────────────────┤
│               Application Layer                     │  ← CQRS Handlers, DTOs, Validation
├─────────────────────────────────────────────────────┤
│                 Domain Layer                        │  ← Entities, Value Objects, Events
└─────────────────────────────────────────────────────┘
         Dependencies point inward only.
         Domain knows nothing about EF Core, HTTP, or JSON.
```

### Layer Responsibilities

**Domain** — The heart of the system. Contains business logic with zero infrastructure dependencies. If a domain class has a `using Microsoft.EntityFrameworkCore` statement, something is wrong.

**Application** — Orchestrates domain objects to fulfill use cases. Handlers read commands/queries, load aggregates through repository interfaces, call domain methods, and commit via Unit of Work. Contains no SQL, no HTTP, no JWT.

**Infrastructure** — Implements the interfaces the Application layer defines. EF Core configurations, repository implementations, JWT token generation, email sending. All the "plumbing."

**API** — Thin HTTP adapter. Controllers translate HTTP requests into commands/queries, pass them to handlers, and translate results into HTTP responses. No business logic lives here.

---

## Domain Model

### Aggregates

```
Customer
  ├── Email (value object)
  └── Address? (value object, nullable shipping address)

Product
  ├── Money Price (value object)
  ├── ProductSku Sku (value object)
  └── ProductVariant[] (child entities)

ShoppingCart  ──────────────────── belongs to Customer (1-to-1)
  └── CartItem[] (child entities)
        └── Money UnitPrice (value object)

Order  ──────────────────────────── placed by Customer
  ├── OrderNumber (value object)
  ├── Address ShippingAddress (value object)
  ├── Money TotalAmount (value object)
  └── OrderItem[] (child entities)
        └── Money UnitPrice (value object, price snapshot)
```

### Value Objects

| Value Object | Validates | Stored As |
|---|---|---|
| `Money` | Amount ≥ 0, valid currency | Two columns: Amount + Currency |
| `Email` | RFC format, normalized to lowercase | Single column |
| `Address` | All fields required | Four columns (Street, City, Country, PostalCode) |
| `ProductSku` | `SHOE-RED-42` format, 4–20 chars | Single column |
| `OrderNumber` | Generated as `ORD-YYYYMMDD-XXXX` | Single column |

### Business Rules (enforced in domain layer)

1. Cannot place an order with insufficient stock
2. Order total must always equal sum of order item line totals (invariant)
3. Cannot cancel a Shipped or Delivered order
4. Stock is reserved the moment an order is placed
5. Customers can only view and modify their own orders
6. Admins can view all orders and manage products
7. Shopping cart items can only be modified by the cart owner
8. Product prices cannot be negative
9. An order must have at least one item

### Order Status Flow

```
Pending → Processing → Shipped → Delivered
                ↓
           Cancelled  (only from Pending or Processing)
```

---

## Project Structure

```
bazario-api/
├── ECommerceApi.Domain/
│   ├── Aggregates/
│   │   ├── Orders/          Order, OrderItem
│   │   ├── Products/        Product, ProductVariant
│   │   ├── Customers/       Customer
│   │   └── Carts/           ShoppingCart, CartItem
│   ├── ValueObjects/        Money, Email, Address, ProductSku, OrderNumber
│   ├── Events/              IDomainEvent, OrderPlacedEvent, OrderCancelledEvent, StockReservedEvent
│   ├── Repositories/        IOrderRepository, IProductRepository, ICustomerRepository, ICartRepository
│   ├── Common/              Entity, AggregateRoot, ValueObject, IUnitOfWork
│   └── Enums/               OrderStatus, Currency
│
├── ECommerceApi.Application/
│   ├── Common/              Result<T>, Error, ICommandHandler, IQueryHandler
│   ├── Orders/              Commands (PlaceOrder, CancelOrder, UpdateStatus), Queries (GetOrder, GetOrders)
│   ├── Products/            Commands (CreateProduct, UpdateProduct, DeleteProduct), Queries (GetProduct, SearchProducts)
│   ├── Customers/           Commands (Register, UpdateProfile, ChangePassword), Queries (GetCustomer)
│   ├── Carts/               Commands (AddItem, UpdateQuantity, RemoveItem, ClearCart), Queries (GetCart)
│   └── Auth/                Commands (Login, RefreshToken)
│
├── ECommerceApi.Infrastructure/
│   ├── Persistence/         AppDbContext, Configurations (Fluent API), Migrations
│   ├── Repositories/        OrderRepository, ProductRepository, CustomerRepository, CartRepository
│   ├── Interceptors/        AuditInterceptor (sets CreatedAt, UpdatedAt automatically)
│   └── Auth/                JwtTokenService, PasswordHasher, RefreshTokenRepository
│
├── ECommerceApi.Api/
│   ├── Controllers/         AuthController, ProductsController, OrdersController, CartsController, CustomersController
│   ├── Middleware/          GlobalExceptionHandler, RequestLogging
│   └── Extensions/          ServiceCollectionExtensions (DI wiring)
│
└── ECommerceApi.Tests/
    ├── Domain/              Value object tests, entity business rule tests (no mocks needed)
    └── Application/         Handler tests with Moq + FluentAssertions
```

---

## API Endpoints (~ 28 endpoints)

### Authentication
| Method | Endpoint | Description | Auth |
|---|---|---|---|
| POST | `/api/auth/register` | Register new customer | Public |
| POST | `/api/auth/login` | Login, receive JWT + refresh token | Public |
| POST | `/api/auth/refresh` | Exchange refresh token for new JWT | Public |

### Products
| Method | Endpoint | Description | Auth |
|---|---|---|---|
| GET | `/api/products` | List products (filter by category, price, name) | Public |
| GET | `/api/products/{id}` | Get product detail | Public |
| POST | `/api/products` | Create product | Admin |
| PUT | `/api/products/{id}` | Update product | Admin |
| PATCH | `/api/products/{id}/price` | Update price only | Admin |
| PATCH | `/api/products/{id}/stock` | Add stock | Admin |
| DELETE | `/api/products/{id}` | Soft delete product | Admin |

### Shopping Cart
| Method | Endpoint | Description | Auth |
|---|---|---|---|
| GET | `/api/cart` | Get current customer's cart | Customer |
| POST | `/api/cart/items` | Add item to cart | Customer |
| PUT | `/api/cart/items/{productId}` | Update item quantity | Customer |
| DELETE | `/api/cart/items/{productId}` | Remove item from cart | Customer |
| DELETE | `/api/cart` | Clear cart | Customer |

### Orders
| Method | Endpoint | Description | Auth |
|---|---|---|---|
| POST | `/api/orders` | Place order from cart | Customer |
| GET | `/api/orders` | Get own order history | Customer |
| GET | `/api/orders/{id}` | Get order detail | Customer (own) |
| DELETE | `/api/orders/{id}` | Cancel order | Customer (own) |
| GET | `/api/admin/orders` | Get all orders | Admin |
| PATCH | `/api/admin/orders/{id}/status` | Update order status | Admin |

### Customers
| Method | Endpoint | Description | Auth |
|---|---|---|---|
| GET | `/api/customers/me` | Get own profile | Customer |
| PUT | `/api/customers/me` | Update own profile | Customer |
| POST | `/api/customers/me/change-password` | Change password | Customer |
| GET | `/api/admin/customers` | List all customers | Admin |
| GET | `/api/admin/customers/{id}` | Get customer detail | Admin |

---

## Getting Started

### Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [Docker Desktop](https://www.docker.com/products/docker-desktop/)

### 1. Start the database

```bash
docker run -e "ACCEPT_EULA=Y" \
           -e "SA_PASSWORD=Bazario_Dev_2025!" \
           -p 1433:1433 \
           --name bazario-sql \
           -d mcr.microsoft.com/mssql/server:2022-latest
```

### 2. Configure the application

```bash
# In ECommerceApi.Api/, create appsettings.Development.json
cp ECommerceApi.Api/appsettings.json ECommerceApi.Api/appsettings.Development.json
```

Update the connection string and JWT settings:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost,1433;Database=BazarioDB;User Id=sa;Password=Bazario_Dev_2025!;TrustServerCertificate=True"
  },
  "Jwt": {
    "SecretKey": "your-secret-key-minimum-32-characters-long",
    "Issuer": "bazario-api",
    "Audience": "bazario-client",
    "ExpiryMinutes": 60,
    "RefreshTokenExpiryDays": 7
  }
}
```

### 3. Run migrations

```bash
cd ECommerceApi.Api
dotnet ef database update --project ../ECommerceApi.Infrastructure
```

### 4. Run the API

```bash
dotnet run --project ECommerceApi.Api
```

Swagger UI available at: `https://localhost:{port}/swagger`

### 5. Run tests

```bash
dotnet test
```

---

## Key Learning Concepts

### Why manual CQRS instead of MediatR?

MediatR is excellent in production, but it hides the pattern behind a library. Building handlers manually forces you to understand what a handler *is* before you let a framework manage it. Once you understand `ICommandHandler<TCommand, TResult>`, MediatR's `IRequestHandler<TRequest, TResponse>` is immediately familiar.

### Why the Result Pattern instead of exceptions?

Exceptions are for *exceptional* situations — unexpected failures. "Customer not found" is not exceptional; it's an expected business outcome. The Result pattern makes expected failure paths explicit in method signatures: `Task<Result<OrderDto>>` tells you immediately that this operation can fail in a predictable way.

### Why internal constructors on child entities?

`OrderItem`'s constructor is `internal`. This means only code within the `ECommerceApi.Domain` project can create an `OrderItem`. The only legitimate creator is `Order.AddItem()` — which validates that the order is in Pending status before creating the item. Without `internal`, nothing stops a developer from newing up an `OrderItem` directly and bypassing all validation.

### What is CORS and why does it matter?

CORS (Cross-Origin Resource Sharing) is a browser security mechanism. When your React frontend at `https://myshop.com` calls your API at `https://api.myshop.com`, the browser blocks the request by default because the origins differ. Your API must explicitly tell the browser which origins are allowed. CORS configuration lives in the API layer and is explained in detail in the API session.

---

## Git Checkpoint History

| Checkpoint | Description |
|---|---|
| A | Complete Domain layer — entities, value objects, aggregates, domain events, repository interfaces |
| B | Repository documentation — README, architecture overview |
| C | Application layer — Result pattern, CQRS abstractions, command/query handlers |
| D | Infrastructure — EF Core, Fluent API configurations, repositories, migrations |
| E | API layer — controllers, middleware, Swagger, CORS |
| F | JWT authentication — register, login, refresh token rotation |
| G | Domain event dispatching — event handlers, stock restoration on cancel |
| H | Unit tests — domain entities, value objects |
| I | Handler tests — Moq + FluentAssertions |
| J | Advanced features — search, filtering, soft delete |
| K | Final polish — end-to-end testing, production hardening |

---

## Tech Stack

| Technology | Version | Purpose |
|---|---|---|
| .NET | 9.0 | Runtime |
| ASP.NET Core | 9.0 | HTTP framework |
| Entity Framework Core | 9.0 | ORM |
| SQL Server | 2022 | Database |
| xUnit | 2.9 | Test framework |
| Moq | 4.20 | Mocking |
| FluentAssertions | 6.12 | Test assertions |
| Swashbuckle | 7.x | Swagger/OpenAPI |

---

## License

MIT — use freely, learn from it, build on it.
