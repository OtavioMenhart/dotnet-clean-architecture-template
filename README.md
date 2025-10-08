# Clean Architecture Template

It implements a solution based on Clean Architecture using .NET 9, focusing on separation of concerns, testability, and scalability. Below is a detailed explanation of the projects in the solution, their roles, and the main technologies used.

---

## Usage of the Template
```
dotnet new install ToolsBR.CleanArchitecture.Template::VERSION
```

Go to your project folder and run:
```
dotnet new clean-arch-tools-br -n YOUR_PROJECT_NAME
```

---

## Solution Structure

The solution is divided into projects according to Clean Architecture principles:

---

### src/CleanArchTemplate.Api/CleanArchTemplate.Api.csproj

**Type:** ASP.NET Core Web API  
**Role:** Presentation layer, responsible for exposing HTTP endpoints, receiving requests, and returning responses.  
**Technologies:**  
- ASP.NET Core
- Serilog (structured logging)
- Swagger/Swashbuckle (API documentation and testing)
- OpenAPI
- Entity Framework Core (Design)
- Dependency injection for Application and Infrastructure

**Highlights:**  
- Serilog configuration for detailed logs.
- OpenTelemetry for observability.
- Swagger for automatic endpoint documentation.
- Middleware for claims validation and global exception handling.
- Automatically applies migrations on startup.

---

### src/CleanArchTemplate.Application/CleanArchTemplate.Application.csproj

**Type:** Class Library  
**Role:** Application layer, containing use cases, business rules, and orchestration between domain and infrastructure.

**Highlights:**  
- Depends only on the domain layer.
- Centralizes handlers, application services, and business logic.

---

### src/CleanArchTemplate.Domain/CleanArchTemplate.Domain.csproj

**Type:** Class Library  
**Role:** Domain layer, responsible for entities, aggregates, interfaces, and pure business rules.  
**Technologies:**  
- Pure C#, no external dependencies

**Highlights:**  
- Defines entities and domain abstractions.
- Does not depend on other layers, ensuring independence and testability.

---

### src/CleanArchTemplate.Infrastructure/CleanArchTemplate.Infrastructure.csproj

**Type:** Class Library  
**Role:** Infrastructure layer, responsible for data persistence, external integrations, and technical implementations.  
**Technologies:**  
- Entity Framework Core (persistence)
- SQL Server
- OpenTelemetry (monitoring)
- RabbitMQ (messaging)

**Highlights:**  
- Implements repositories, database context, and messaging integrations.
- Observability with OpenTelemetry.
- Depends on the domain layer.

---

### src/CleanArchTemplate.Workers/CleanArchTemplate.Workers.csproj

**Type:** .NET Worker Service  
**Role:** Background services for asynchronous processing, such as RabbitMQ event consumers.  
**Technologies:**
- Serilog (logging)
- OpenTelemetry (monitoring)
- RabbitMQ

**Highlights:**  
- Implements event consumers like `EntityEventConsumer`. Uses the nuget https://www.nuget.org/packages/RabbitMq.Messaging.Toolkit
- Runs background tasks decoupled from the API.

---

### tests/CleanArchTemplate.UnitTests/CleanArchTemplate.UnitTests.csproj

**Type:** Unit Tests  
**Role:** Test isolated units of the application, especially business rules and services.  
**Technologies:**  
- xUnit (test framework)
- Moq (mocking)
- AutoFixture (data generation)
- Entity Framework Core InMemory (in-memory database for tests)
- coverlet.collector (code coverage)

**Highlights:**  
- Fast and isolated tests.
- Uses in-memory database to simulate persistence.

---

### tests/CleanArchTemplate.IntegrationTests/CleanArchTemplate.IntegrationTests.csproj

**Type:** Integration Tests  
**Role:** Test integration between components, such as API, database, and messaging.  
**Technologies:**  
- xUnit
- Microsoft.AspNetCore.Mvc.Testing (API testing)
- Entity Framework Core InMemory
- Testcontainers.RabbitMq (RabbitMQ in container for tests)
- coverlet.collector

**Highlights:**  
- Tests that validate the joint operation of modules.
- Uses containers to simulate real environments.

---

### tests/CleanArchTemplate.LoadTests/CleanArchTemplate.LoadTests.csproj

**Type:** Load Tests  
**Role:** Perform load and performance testing against the API and background services.  
**Technologies:**  
- NBomber (load testing framework)
- NBomber.Http (HTTP load testing)
- .NET 9

**Highlights:**  
- Simulates concurrent requests and load scenarios.
- References the Application layer for realistic test orchestration.

---

## Technologies Used

- **.NET 9 / C# 13.0**
- **ASP.NET Core**
- **Entity Framework Core**
- **RabbitMQ**
- **Serilog**
- **OpenTelemetry**
- **Swagger/Swashbuckle**
- **xUnit, Moq, AutoFixture**
- **Docker (via docker-compose for local infrastructure)**

---

## How to run

1. Start the required containers:
```
   docker-compose up -d
```

2. To run the API and Workers together, use the profile:
```
   Api + Workers
```
This profile ensures all necessary services are executed in an integrated environment.

3. To run the API, Workers, and Load Tests together, use the profile:
```
   Api + Workers + Load Tests
```
This profile ensures all necessary services and load tests are executed in an integrated environment.

---

This structure ensures separation of concerns, ease of maintenance, and scalability, following Clean Architecture principles.