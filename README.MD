# CQRS Demo API

This project demonstrates CQRS (Command Query Responsibility Segregation) and Domain-Driven Design (DDD) using .NET 9, MediatR, and EF Core. It provides CRUD operations for managing Order entities.

---

## Features

1. Separation of commands (write) and queries (read) using CQRS.
2. Domain-Driven Design (DDD) for business logic encapsulation.
3. Swagger/OpenAPI for API documentation and testing.
4. EF Core with an in-memory database for simplicity.
5. Dockerized deployment.

---

## Prerequisites

1. .NET 9 SDK
2. Docker Desktop
3. A code editor like Visual Studio or Visual Studio Code

---

## Installation and Setup

1. Clone the repository:
   git clone https://github.com/your-repo/cqrs-demo-api.git
   cd cqrs-demo-api

2. Build and run locally:

   Using .NET CLI:
   - dotnet restore
   - dotnet build
   - dotnet run --project CqrsDemo.Api
   - Swagger UI available at: http://localhost:5000/swagger

   Using Docker:
   - docker build -t cqrsdemoapi:dev .
   - docker run -p 8080:80 cqrsdemoapi:dev
   - Swagger UI available at: http://localhost:8080/swagger

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

1. Build the image:
   docker build -t cqrsdemoapi:dev .

2. Run the container:
   docker run -p 8080:80 cqrsdemoapi:dev

3. Access Swagger UI:
   http://localhost:8080/swagger
