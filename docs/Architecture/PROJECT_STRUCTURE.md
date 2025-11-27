# Estructura del Proyecto - Clean Architecture

Este documento describe la estructura de carpetas del proyecto basado en los principios de Clean Architecture.

## 📁 Estructura General

```
service-api-csharp/
├── 📂 service-api-csharp.Domain/          # Capa de Dominio (núcleo del negocio)
├── 📂 service-api-csharp.Application/     # Capa de Aplicación (casos de uso)
├── 📂 service-api-csharp.Infrastructure/  # Capa de Infraestructura (implementaciones)
├── 📂 service-api-csharp.API/             # Capa de Presentación (API REST)
├── 📂 tests/                              # Pruebas automatizadas
├── 📂 docs/                               # Documentación del proyecto
├── 📂 docker/                             # Configuración de Docker
└── 📂 scripts/                            # Scripts de utilidad
```

## 🎯 Capa de Dominio (Domain)

**Responsabilidad**: Contiene la lógica de negocio central y las reglas del dominio. No tiene dependencias de otras capas.

```
service-api-csharp.Domain/
├── Entities/              # Entidades del dominio con identidad única
├── ValueObjects/          # Objetos de valor inmutables
├── Enums/                 # Enumeraciones del dominio
├── Events/                # Eventos del dominio (Domain Events)
├── Exceptions/            # Excepciones específicas del dominio
├── Interfaces/            # Interfaces de repositorios y servicios
├── Specifications/        # Especificaciones para consultas complejas
└── Aggregates/            # Agregados (raíces de agregado)
```

### Descripción de carpetas:
- **Entities**: Clases que representan objetos con identidad única (ej: `User`, `Product`, `Order`)
- **ValueObjects**: Objetos inmutables definidos por sus atributos (ej: `Address`, `Money`, `Email`)
- **Enums**: Enumeraciones del dominio (ej: `OrderStatus`, `UserRole`)
- **Events**: Eventos que ocurren en el dominio (ej: `OrderCreatedEvent`, `UserRegisteredEvent`)
- **Exceptions**: Excepciones personalizadas del dominio (ej: `InvalidEmailException`)
- **Interfaces**: Contratos para repositorios y servicios (ej: `IProductRepository`)
- **Specifications**: Patrones de especificación para consultas complejas
- **Aggregates**: Raíces de agregado que agrupan entidades relacionadas

## 🔧 Capa de Aplicación (Application)

**Responsabilidad**: Contiene la lógica de aplicación, casos de uso y orquestación. Depende solo de la capa de Dominio.

```
service-api-csharp.Application/
├── Features/              # Características organizadas por funcionalidad (CQRS)
├── Common/                # Componentes compartidos
│   ├── Behaviors/         # Comportamientos de MediatR (validación, logging, etc.)
│   ├── Interfaces/        # Interfaces de servicios de aplicación
│   ├── Mappings/          # Perfiles de AutoMapper
│   ├── Models/            # Modelos compartidos
│   ├── Exceptions/        # Excepciones de aplicación
│   └── Validators/        # Validadores de FluentValidation
├── DTOs/                  # Data Transfer Objects
├── UseCases/              # Casos de uso de la aplicación
└── Products/              # Ejemplo de feature (puede moverse a Features/)
```

### Descripción de carpetas:
- **Features**: Organización vertical por característica (ej: `Products/`, `Users/`, `Orders/`)
  - Cada feature contiene sus Commands, Queries, Handlers, DTOs y Validators
- **Common/Behaviors**: Pipelines de MediatR (ej: `ValidationBehavior`, `LoggingBehavior`)
- **Common/Interfaces**: Interfaces de servicios (ej: `IEmailService`, `ICurrentUserService`)
- **Common/Mappings**: Configuración de AutoMapper
- **Common/Validators**: Validadores base y compartidos
- **DTOs**: Objetos de transferencia de datos
- **UseCases**: Casos de uso específicos de la aplicación

## 🏗️ Capa de Infraestructura (Infrastructure)

**Responsabilidad**: Implementa las interfaces definidas en Domain y Application. Contiene detalles técnicos.

```
service-api-csharp.Infrastructure/
├── Persistence/           # Acceso a datos
│   ├── Configurations/    # Configuraciones de Entity Framework
│   ├── Migrations/        # Migraciones de base de datos
│   └── Interceptors/      # Interceptores de EF Core
├── Repositories/          # Implementaciones de repositorios
├── Services/              # Implementaciones de servicios
├── Identity/              # Autenticación y autorización
├── Logging/               # Implementación de logging
├── Caching/               # Implementación de caché
├── Email/                 # Servicio de correo electrónico
├── FileStorage/           # Almacenamiento de archivos
└── ExternalServices/      # Integraciones con servicios externos
```

### Descripción de carpetas:
- **Persistence/Configurations**: Configuraciones de entidades con Fluent API
- **Persistence/Migrations**: Migraciones de Entity Framework Core
- **Persistence/Interceptors**: Interceptores para auditoría, soft delete, etc.
- **Repositories**: Implementaciones de `IRepository<T>`
- **Services**: Servicios de infraestructura (ej: `DateTimeService`, `FileService`)
- **Identity**: Implementación de autenticación con JWT, Identity, etc.
- **Logging**: Configuración de Serilog, Application Insights, etc.
- **Caching**: Implementación de Redis, Memory Cache, etc.
- **Email**: Servicios de envío de correos (SendGrid, SMTP)
- **FileStorage**: Azure Blob Storage, AWS S3, sistema de archivos local
- **ExternalServices**: Clientes para APIs externas

## 🌐 Capa de Presentación (API)

**Responsabilidad**: Punto de entrada de la aplicación. Expone endpoints REST y maneja HTTP.

```
service-api-csharp.API/
├── Controllers/           # Controladores de API
├── Middleware/            # Middleware personalizado
├── Filters/               # Filtros de acción y excepciones
├── Extensions/            # Métodos de extensión para configuración
├── Configuration/         # Archivos de configuración
└── Properties/            # Propiedades del proyecto
```

### Descripción de carpetas:
- **Controllers**: Controladores REST que exponen endpoints
- **Middleware**: Middleware personalizado (ej: `ExceptionHandlingMiddleware`)
- **Filters**: Filtros de acción, autorización y excepciones
- **Extensions**: Extensiones para configurar servicios (ej: `ServiceCollectionExtensions`)
- **Configuration**: Configuración de Swagger, CORS, etc.

## 🧪 Pruebas (Tests)

**Responsabilidad**: Contiene todas las pruebas automatizadas del proyecto.

```
tests/
├── UnitTests/             # Pruebas unitarias
│   ├── Domain/            # Pruebas de la capa de dominio
│   ├── Application/       # Pruebas de la capa de aplicación
│   ├── Infrastructure/    # Pruebas de la capa de infraestructura
│   └── API/               # Pruebas de controladores
├── IntegrationTests/      # Pruebas de integración
└── E2ETests/              # Pruebas end-to-end
```

### Descripción de carpetas:
- **UnitTests**: Pruebas unitarias aisladas con mocks
- **IntegrationTests**: Pruebas que verifican la integración entre componentes
- **E2ETests**: Pruebas de extremo a extremo del flujo completo

## 📚 Documentación (Docs)

```
docs/
├── Architecture/          # Documentación de arquitectura
├── API/                   # Documentación de la API
├── Diagrams/              # Diagramas del sistema
└── Guides/                # Guías de desarrollo
```

## 🐳 Docker

```
docker/
└── (Archivos de configuración Docker, docker-compose, etc.)
```

## 📜 Scripts

```
scripts/
└── (Scripts de utilidad para build, deployment, etc.)
```

## 🔄 Flujo de Dependencias

```
API → Application → Domain
  ↓         ↓
Infrastructure
```

### Reglas de dependencia:
1. **Domain**: No depende de ninguna otra capa (núcleo puro)
2. **Application**: Solo depende de Domain
3. **Infrastructure**: Depende de Domain y Application (implementa interfaces)
4. **API**: Depende de todas las capas (punto de entrada)

## 📦 Paquetes NuGet Recomendados

### Domain
- Ninguno (debe ser puro)

### Application
- `MediatR`
- `AutoMapper`
- `FluentValidation`

### Infrastructure
- `Microsoft.EntityFrameworkCore`
- `Microsoft.EntityFrameworkCore.SqlServer` o `Npgsql.EntityFrameworkCore.PostgreSQL`
- `Serilog`
- `StackExchange.Redis` (para caching)

### API
- `Microsoft.AspNetCore.Authentication.JwtBearer`
- `Swashbuckle.AspNetCore` (Swagger)
- `FluentValidation.AspNetCore`

## 🎯 Principios Aplicados

1. **Separation of Concerns**: Cada capa tiene una responsabilidad específica
2. **Dependency Inversion**: Las capas externas dependen de las internas
3. **Single Responsibility**: Cada clase tiene una única razón para cambiar
4. **Open/Closed**: Abierto para extensión, cerrado para modificación
5. **Interface Segregation**: Interfaces específicas en lugar de generales
6. **DRY (Don't Repeat Yourself)**: Código reutilizable en Common

## 📝 Convenciones de Nomenclatura

- **Entidades**: PascalCase, singular (ej: `Product`, `User`)
- **Interfaces**: Prefijo `I` + PascalCase (ej: `IProductRepository`)
- **DTOs**: Sufijo `Dto` (ej: `ProductDto`, `CreateProductDto`)
- **Commands**: Sufijo `Command` (ej: `CreateProductCommand`)
- **Queries**: Sufijo `Query` (ej: `GetProductByIdQuery`)
- **Handlers**: Sufijo `Handler` (ej: `CreateProductCommandHandler`)
- **Validators**: Sufijo `Validator` (ej: `CreateProductCommandValidator`)

## 🚀 Próximos Pasos

1. Implementar entidades del dominio
2. Crear interfaces de repositorios
3. Implementar casos de uso con MediatR
4. Configurar Entity Framework Core
5. Implementar repositorios
6. Crear controladores de API
7. Configurar autenticación y autorización
8. Implementar pruebas unitarias
9. Configurar CI/CD

---

**Última actualización**: 2025-11-26
