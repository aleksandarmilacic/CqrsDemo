# CQRS Demo API

This project demonstrates CQRS (Command Query Responsibility Segregation) and Domain-Driven Design (DDD) using .NET 9, MediatR, and EF Core. It provides CRUD operations for managing Order entities.

---

## Features

- CQRS Architecture: Clear separation of commands (write operations) and queries (read operations).
- Domain-Driven Design (DDD): Encapsulation of business logic within the domain layer.
- Event-Driven Architecture: Integration with RabbitMQ for event publishing and subscribing.
- Caching: Redis caching for improved read performance and scalability.
- API Documentation: Swagger/OpenAPI support for API exploration and testing.
- EF Core with In-Memory Database: Simplified setup for demonstration and testing.
- Dockerized Deployment: Ready-to-use Docker containers for quick deployment.
- Polly for Resilience: Retry and circuit breaker policies for fault tolerance.

---

## Prerequisites

- .NET 9 SDK
- Docker Desktop
- A code editor like Visual Studio or Visual Studio Code
- RabbitMQ and Redis (can be run via Docker)

---


## API Endpoints

1. POST /api/orders - Create an order
2. GET /api/orders - Get all orders
3. GET /api/orders/{id} - Get an order by ID
4. PUT /api/orders/{id} - Update an order
5. DELETE /api/orders/{id} - Delete an order
6. POST /api/orders/calculate-discount - Business logic example

---

## Deployment with Docker

docker-compose up
