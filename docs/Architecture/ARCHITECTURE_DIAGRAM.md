# Clean Architecture - Diagrama de Capas

## Diagrama de Dependencias

```
┌─────────────────────────────────────────────────────────────────┐
│                         API Layer                                │
│  ┌──────────────────────────────────────────────────────────┐   │
│  │  Controllers  │  Middleware  │  Filters  │  Extensions   │   │
│  └──────────────────────────────────────────────────────────┘   │
└────────────────────────────┬────────────────────────────────────┘
                             │
                             ▼
┌─────────────────────────────────────────────────────────────────┐
│                    Application Layer                             │
│  ┌──────────────────────────────────────────────────────────┐   │
│  │  Features (CQRS)  │  DTOs  │  Validators  │  Mappings   │   │
│  │  Commands  │  Queries  │  Handlers  │  Behaviors        │   │
│  └──────────────────────────────────────────────────────────┘   │
└────────────────────────────┬────────────────────────────────────┘
                             │
                             ▼
┌─────────────────────────────────────────────────────────────────┐
│                      Domain Layer (Core)                         │
│  ┌──────────────────────────────────────────────────────────┐   │
│  │  Entities  │  Value Objects  │  Aggregates  │  Events   │   │
│  │  Interfaces  │  Specifications  │  Enums  │  Exceptions │   │
│  └──────────────────────────────────────────────────────────┘   │
└─────────────────────────────────────────────────────────────────┘
                             ▲
                             │
┌────────────────────────────┴────────────────────────────────────┐
│                   Infrastructure Layer                           │
│  ┌──────────────────────────────────────────────────────────┐   │
│  │  Persistence  │  Repositories  │  Identity  │  Services  │   │
│  │  Caching  │  Email  │  Logging  │  External Services    │   │
│  └──────────────────────────────────────────────────────────┘   │
└─────────────────────────────────────────────────────────────────┘
```

## Flujo de una Petición HTTP

```
1. HTTP Request
   │
   ▼
2. API Controller
   │
   ▼
3. MediatR Command/Query
   │
   ▼
4. Validation Behavior (FluentValidation)
   │
   ▼
5. Command/Query Handler (Application)
   │
   ├──▶ Domain Entities (Business Logic)
   │
   └──▶ Repository Interface (Domain)
        │
        ▼
6. Repository Implementation (Infrastructure)
   │
   ▼
7. Database (EF Core)
   │
   ▼
8. Response (DTO)
   │
   ▼
9. HTTP Response
```

## Organización por Feature (Vertical Slice)

```
Features/
└── Products/
    ├── Commands/
    │   ├── CreateProduct/
    │   │   ├── CreateProductCommand.cs
    │   │   ├── CreateProductCommandHandler.cs
    │   │   └── CreateProductCommandValidator.cs
    │   └── UpdateProduct/
    │       ├── UpdateProductCommand.cs
    │       ├── UpdateProductCommandHandler.cs
    │       └── UpdateProductCommandValidator.cs
    ├── Queries/
    │   ├── GetProductById/
    │   │   ├── GetProductByIdQuery.cs
    │   │   └── GetProductByIdQueryHandler.cs
    │   └── GetProductsList/
    │       ├── GetProductsListQuery.cs
    │       └── GetProductsListQueryHandler.cs
    └── DTOs/
        ├── ProductDto.cs
        ├── CreateProductDto.cs
        └── UpdateProductDto.cs
```

## Patrón CQRS (Command Query Responsibility Segregation)

```
┌─────────────────────────────────────────────────────────────────┐
│                          Client                                  │
└────────────┬────────────────────────────────────┬────────────────┘
             │                                    │
             │ Commands (Write)                   │ Queries (Read)
             │ - Create                           │ - Get by ID
             │ - Update                           │ - Get List
             │ - Delete                           │ - Search
             ▼                                    ▼
┌────────────────────────┐          ┌────────────────────────────┐
│   Command Handlers     │          │    Query Handlers          │
│  - Modify state        │          │  - Read-only               │
│  - Validation          │          │  - No side effects         │
│  - Business rules      │          │  - Optimized for reads     │
└────────┬───────────────┘          └────────┬───────────────────┘
         │                                   │
         ▼                                   ▼
┌────────────────────────┐          ┌────────────────────────────┐
│   Write Database       │          │    Read Database           │
│   (Normalized)         │◀─────────│    (Denormalized/Cache)    │
└────────────────────────┘   Sync   └────────────────────────────┘
```

## Inyección de Dependencias

```
Program.cs (API)
│
├──▶ AddDomainServices()
│    └── Register domain services (if any)
│
├──▶ AddApplicationServices()
│    ├── MediatR
│    ├── AutoMapper
│    ├── FluentValidation
│    └── Behaviors
│
├──▶ AddInfrastructureServices()
│    ├── DbContext
│    ├── Repositories
│    ├── Identity
│    ├── Caching
│    └── External Services
│
└──▶ AddApiServices()
     ├── Controllers
     ├── Swagger
     ├── CORS
     └── Authentication
```

## Principios SOLID Aplicados

### Single Responsibility Principle (SRP)
- Cada clase tiene una única responsabilidad
- Handlers manejan un solo comando/query

### Open/Closed Principle (OCP)
- Extensible mediante nuevos handlers
- Cerrado para modificación mediante interfaces

### Liskov Substitution Principle (LSP)
- Las implementaciones pueden sustituir interfaces
- Repositorios intercambiables

### Interface Segregation Principle (ISP)
- Interfaces específicas por funcionalidad
- No interfaces "gordas"

### Dependency Inversion Principle (DIP)
- Dependencias hacia abstracciones
- Inyección de dependencias

## Ejemplo de Flujo Completo

### Crear un Producto

```
1. POST /api/products
   Body: { "name": "Product 1", "price": 100 }

2. ProductsController.Create()
   └──▶ _mediator.Send(new CreateProductCommand(...))

3. ValidationBehavior<CreateProductCommand>
   └──▶ CreateProductCommandValidator.Validate()

4. CreateProductCommandHandler.Handle()
   ├──▶ var product = Product.Create(...)  // Domain Entity
   ├──▶ await _productRepository.AddAsync(product)
   └──▶ await _unitOfWork.SaveChangesAsync()

5. Return ProductDto (mapped by AutoMapper)

6. HTTP 201 Created
   Body: { "id": 1, "name": "Product 1", "price": 100 }
```

## Tecnologías Recomendadas

### Domain
- ✅ C# 12+
- ✅ .NET 8+

### Application
- ✅ MediatR (CQRS)
- ✅ AutoMapper (Mapping)
- ✅ FluentValidation (Validation)

### Infrastructure
- ✅ Entity Framework Core (ORM)
- ✅ PostgreSQL / SQL Server (Database)
- ✅ Redis (Caching)
- ✅ Serilog (Logging)
- ✅ JWT (Authentication)

### API
- ✅ ASP.NET Core Web API
- ✅ Swagger/OpenAPI (Documentation)
- ✅ Minimal APIs (opcional)

### Testing
- ✅ xUnit / NUnit
- ✅ Moq / NSubstitute
- ✅ FluentAssertions
- ✅ Testcontainers (Integration Tests)

---

**Última actualización**: 2025-11-26
