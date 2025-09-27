# My Project

This project is based on Clean Architecture principles and targets .NET 9.

## Getting Started

1. Start required services using Docker Compose:

```
docker-compose up -d
```

2. To add a new migration:

```
dotnet ef migrations add AddProductTable --project src/MyProject.Infrastructure --startup-project src/MyProject.Api --output-dir Persistence/Migrations --context AppDbContext
```

Migrations are applied automatically when the application starts.

## Solution Structure

- **src/MyProject.Api**: ASP.NET Core Web API (entry point)
- **src/MyProject.Application**: Application layer (business logic, use cases)
- **src/MyProject.Domain**: Domain layer (entities, interfaces)
- **src/MyProject.Infrastructure**: Infrastructure layer (data access, integrations)
- **src/MyProject.Workers**: Background worker services
- **tests/MyProject.UnitTests**: Unit tests
- **tests/MyProject.IntegrationTests**: Integration tests

## Notes

- The solution uses Docker Compose for local infrastructure.
- .NET 9 and C# 13.0 are required.