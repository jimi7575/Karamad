# Karamad Bookstore Microservices

Two ASP.NET Core 8 microservices implementing a simple bookstore order flow with Clean Architecture, MediatR, SQL Server, Redis, RabbitMQ, and Docker Compose.

## Services

- Catalog API: `http://localhost:5001/swagger`
- Order API: `http://localhost:5002/swagger`
- RabbitMQ Management: `http://localhost:15672` (`guest` / `guest`)

## Run

```powershell
docker compose up --build
```

## Main Flow

1. Create a book in Catalog:

```http
POST http://localhost:5001/api/books
Content-Type: application/json

{
  "title": "Clean Architecture",
  "author": "Robert C. Martin",
  "stock": 5,
  "price": 42.5
}
```

2. Read a book by id through Redis cache-aside:

```http
GET http://localhost:5001/api/books/{bookId}
```

3. Create an order:

```http
POST http://localhost:5002/api/orders
Content-Type: application/json

{
  "bookId": "{bookId}",
  "quantity": 2
}
```

The Order service saves the order as `Pending` and publishes `OrderCreatedEvent`. Catalog consumes it, reserves stock if possible, then publishes `StockReservedEvent` or `StockFailedEvent`. Order consumes the final event and moves the order to `Confirmed` or `Failed`.

## Resiliency Notes

- RabbitMQ exchange and queues are durable.
- Messages are published as persistent.
- Consumers use manual acknowledgements.
- If Catalog is down, `OrderCreatedEvent` remains in the durable queue and is processed when Catalog starts again.
- Order uses Polly retry while applying final stock result events.
