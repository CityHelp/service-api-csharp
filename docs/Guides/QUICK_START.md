# Guía de Inicio Rápido - Clean Architecture

Esta guía te ayudará a comenzar a desarrollar en este proyecto de Clean Architecture.

## 🚀 Inicio Rápido

### 1. Verificar Requisitos

```bash
# Verificar versión de .NET
dotnet --version
# Debe ser 8.0 o superior

# Verificar PostgreSQL (o SQL Server)
psql --version
```

### 2. Restaurar Dependencias

```bash
# Restaurar paquetes NuGet
dotnet restore
```

### 3. Configurar Base de Datos

Edita `service-api-csharp.API/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=servicedb;Username=postgres;Password=tu_password"
  }
}
```

### 4. Ejecutar la Aplicación

```bash
cd service-api-csharp.API
dotnet run
```

Accede a Swagger: `https://localhost:5001/swagger`

## 📝 Crear tu Primera Feature

### Paso 1: Crear la Entidad (Domain)

**Ubicación**: `service-api-csharp.Domain/Entities/Product.cs`

```csharp
namespace ServiceApiCsharp.Domain.Entities;

public class Product
{
    public int Id { get; private set; }
    public string Name { get; private set; }
    public string Description { get; private set; }
    public decimal Price { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    private Product() { } // Para EF Core

    public static Product Create(string name, string description, decimal price)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name is required", nameof(name));
        
        if (price <= 0)
            throw new ArgumentException("Price must be greater than zero", nameof(price));

        return new Product
        {
            Name = name,
            Description = description,
            Price = price,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void Update(string name, string description, decimal price)
    {
        Name = name;
        Description = description;
        Price = price;
        UpdatedAt = DateTime.UtcNow;
    }
}
```

### Paso 2: Crear la Interfaz del Repositorio (Domain)

**Ubicación**: `service-api-csharp.Domain/Interfaces/IProductRepository.cs`

```csharp
namespace ServiceApiCsharp.Domain.Interfaces;

public interface IProductRepository
{
    Task<Product?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<List<Product>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Product> AddAsync(Product product, CancellationToken cancellationToken = default);
    Task UpdateAsync(Product product, CancellationToken cancellationToken = default);
    Task DeleteAsync(Product product, CancellationToken cancellationToken = default);
}
```

### Paso 3: Crear DTOs (Application)

**Ubicación**: `service-api-csharp.Application/Features/Products/DTOs/ProductDto.cs`

```csharp
namespace ServiceApiCsharp.Application.Features.Products.DTOs;

public record ProductDto
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public decimal Price { get; init; }
    public DateTime CreatedAt { get; init; }
}

public record CreateProductDto
{
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public decimal Price { get; init; }
}
```

### Paso 4: Crear Command (Application)

**Ubicación**: `service-api-csharp.Application/Features/Products/Commands/CreateProduct/CreateProductCommand.cs`

```csharp
using MediatR;

namespace ServiceApiCsharp.Application.Features.Products.Commands.CreateProduct;

public record CreateProductCommand(
    string Name,
    string Description,
    decimal Price
) : IRequest<ProductDto>;
```

### Paso 5: Crear Handler (Application)

**Ubicación**: `service-api-csharp.Application/Features/Products/Commands/CreateProduct/CreateProductCommandHandler.cs`

```csharp
using MediatR;
using AutoMapper;
using ServiceApiCsharp.Domain.Entities;
using ServiceApiCsharp.Domain.Interfaces;

namespace ServiceApiCsharp.Application.Features.Products.Commands.CreateProduct;

public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, ProductDto>
{
    private readonly IProductRepository _repository;
    private readonly IMapper _mapper;

    public CreateProductCommandHandler(IProductRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<ProductDto> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var product = Product.Create(request.Name, request.Description, request.Price);
        
        var createdProduct = await _repository.AddAsync(product, cancellationToken);
        
        return _mapper.Map<ProductDto>(createdProduct);
    }
}
```

### Paso 6: Crear Validator (Application)

**Ubicación**: `service-api-csharp.Application/Features/Products/Commands/CreateProduct/CreateProductCommandValidator.cs`

```csharp
using FluentValidation;

namespace ServiceApiCsharp.Application.Features.Products.Commands.CreateProduct;

public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required")
            .MaximumLength(100).WithMessage("Name must not exceed 100 characters");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Description must not exceed 500 characters");

        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("Price must be greater than zero");
    }
}
```

### Paso 7: Implementar Repositorio (Infrastructure)

**Ubicación**: `service-api-csharp.Infrastructure/Repositories/ProductRepository.cs`

```csharp
using Microsoft.EntityFrameworkCore;
using ServiceApiCsharp.Domain.Entities;
using ServiceApiCsharp.Domain.Interfaces;
using ServiceApiCsharp.Infrastructure.Persistence;

namespace ServiceApiCsharp.Infrastructure.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly ApplicationDbContext _context;

    public ProductRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Product?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Products
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<List<Product>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Products.ToListAsync(cancellationToken);
    }

    public async Task<Product> AddAsync(Product product, CancellationToken cancellationToken = default)
    {
        await _context.Products.AddAsync(product, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return product;
    }

    public async Task UpdateAsync(Product product, CancellationToken cancellationToken = default)
    {
        _context.Products.Update(product);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Product product, CancellationToken cancellationToken = default)
    {
        _context.Products.Remove(product);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
```

### Paso 8: Crear Controller (API)

**Ubicación**: `service-api-csharp.API/Controllers/ProductsController.cs`

```csharp
using MediatR;
using Microsoft.AspNetCore.Mvc;
using ServiceApiCsharp.Application.Features.Products.Commands.CreateProduct;
using ServiceApiCsharp.Application.Features.Products.DTOs;

namespace ServiceApiCsharp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProductsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [ProducesResponseType(typeof(ProductDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ProductDto>> Create([FromBody] CreateProductDto dto)
    {
        var command = new CreateProductCommand(dto.Name, dto.Description, dto.Price);
        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ProductDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProductDto>> GetById(int id)
    {
        // Implementar GetProductByIdQuery
        return Ok();
    }
}
```

## 🔧 Configuración de Servicios

### Program.cs (API)

```csharp
using ServiceApiCsharp.Infrastructure;
using ServiceApiCsharp.Application;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add Application services (MediatR, AutoMapper, FluentValidation)
builder.Services.AddApplicationServices();

// Add Infrastructure services (DbContext, Repositories)
builder.Services.AddInfrastructureServices(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
```

## 📦 Instalar Paquetes NuGet

```bash
# Application Layer
cd service-api-csharp.Application
dotnet add package MediatR
dotnet add package AutoMapper.Extensions.Microsoft.DependencyInjection
dotnet add package FluentValidation.DependencyInjectionExtensions

# Infrastructure Layer
cd ../service-api-csharp.Infrastructure
dotnet add package Microsoft.EntityFrameworkCore
dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL
dotnet add package Microsoft.EntityFrameworkCore.Tools

# API Layer
cd ../service-api-csharp.API
dotnet add package Swashbuckle.AspNetCore
dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer
```

## 🧪 Crear Prueba Unitaria

**Ubicación**: `tests/UnitTests/Application/Products/CreateProductCommandHandlerTests.cs`

```csharp
using Xunit;
using Moq;
using AutoMapper;
using ServiceApiCsharp.Application.Features.Products.Commands.CreateProduct;
using ServiceApiCsharp.Domain.Interfaces;

namespace UnitTests.Application.Products;

public class CreateProductCommandHandlerTests
{
    [Fact]
    public async Task Handle_ValidCommand_ReturnsProductDto()
    {
        // Arrange
        var mockRepo = new Mock<IProductRepository>();
        var mockMapper = new Mock<IMapper>();
        var handler = new CreateProductCommandHandler(mockRepo.Object, mockMapper.Object);
        var command = new CreateProductCommand("Test Product", "Description", 99.99m);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        mockRepo.Verify(r => r.AddAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}
```

## 📚 Recursos Adicionales

- [Documentación completa](../docs/Architecture/PROJECT_STRUCTURE.md)
- [Diagramas de arquitectura](../docs/Architecture/ARCHITECTURE_DIAGRAM.md)
- [MediatR Documentation](https://github.com/jbogard/MediatR/wiki)
- [FluentValidation Documentation](https://docs.fluentvalidation.net/)
- [AutoMapper Documentation](https://docs.automapper.org/)

## 🎯 Checklist de Desarrollo

- [ ] Crear entidad en Domain
- [ ] Crear interfaz de repositorio en Domain
- [ ] Crear DTOs en Application
- [ ] Crear Command/Query en Application
- [ ] Crear Handler en Application
- [ ] Crear Validator en Application
- [ ] Implementar repositorio en Infrastructure
- [ ] Crear controller en API
- [ ] Crear pruebas unitarias
- [ ] Probar endpoint en Swagger

---

**¡Feliz codificación!** 🚀
