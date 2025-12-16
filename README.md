# Service API - Clean Architecture

[![.NET](https://img.shields.io/badge/.NET-8.0-purple)](https://dotnet.microsoft.com/)
[![C#](https://img.shields.io/badge/C%23-12.0-blue)](https://docs.microsoft.com/en-us/dotnet/csharp/)
[![License](https://img.shields.io/badge/license-MIT-green)](LICENSE)

API REST construida con .NET 8 siguiendo los principios de Clean Architecture, CQRS y Domain-Driven Design (DDD).
Haciendo pruebas para ver si funciona en Actions...

## 🏗️ Arquitectura

Este proyecto implementa **Clean Architecture** con las siguientes capas:

```
┌─────────────────────────────────────────┐
│            API Layer                    │  ← Presentación (Controllers, Middleware)
├─────────────────────────────────────────┤
│        Application Layer                │  ← Casos de Uso (CQRS, Handlers)
├─────────────────────────────────────────┤
│         Domain Layer                    │  ← Lógica de Negocio (Entities, Rules)
├─────────────────────────────────────────┤
│      Infrastructure Layer               │  ← Implementaciones (DB, Services)
└─────────────────────────────────────────┘
```
Testeando cambios para ver si funciona
### Principios Aplicados

- ✅ **Clean Architecture**: Separación de responsabilidades en capas
- ✅ **CQRS**: Separación de comandos y consultas
- ✅ **DDD**: Domain-Driven Design con entidades ricas
- ✅ **SOLID**: Principios de diseño orientado a objetos
- ✅ **Repository Pattern**: Abstracción del acceso a datos
- ✅ **Unit of Work**: Gestión de transacciones
- ✅ **Dependency Injection**: Inversión de dependencias

## 📁 Estructura del Proyecto

```
service-api-csharp/
├── service-api-csharp.Domain/          # Entidades, Value Objects, Interfaces
├── service-api-csharp.Application/     # Casos de Uso, DTOs, Validators
├── service-api-csharp.Infrastructure/  # EF Core, Repositories, Services
├── service-api-csharp.API/             # Controllers, Middleware, Filters
├── tests/                              # Pruebas (Unit, Integration, E2E)
├── docs/                               # Documentación
├── docker/                             # Configuración Docker
└── scripts/                            # Scripts de utilidad
```

Para más detalles, consulta la [documentación de la estructura](docs/Architecture/PROJECT_STRUCTURE.md).

## 🚀 Tecnologías

### Backend
- **.NET 8** - Framework principal
- **ASP.NET Core** - Web API
- **Entity Framework Core** - ORM
- **MediatR** - Patrón Mediator para CQRS
- **AutoMapper** - Mapeo de objetos
- **FluentValidation** - Validación de datos

### Base de Datos
- **PostgreSQL** / **SQL Server** - Base de datos relacional

### Autenticación y Seguridad
- **JWT** - JSON Web Tokens
- **ASP.NET Core Identity** - Gestión de usuarios

### Documentación
- **Swagger/OpenAPI** - Documentación de API

### Testing
- **FluentAssertions** - Assertions fluidas

### DevOps
- **Docker** - Contenedorización
- **GitHub Actions** - CI/CD

## 📋 Requisitos Previos

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [PostgreSQL](https://www.postgresql.org/download/) o [SQL Server](https://www.microsoft.com/sql-server/sql-server-downloads)
- [Docker](https://www.docker.com/get-started) (opcional)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) o [VS Code](https://code.visualstudio.com/)

## 🛠️ Instalación y Configuración

### 1. Clonar el repositorio

```bash
git clone https://github.com/tu-usuario/service-api-csharp.git
cd service-api-csharp
```

### 2. Configurar la base de datos

Edita el archivo `appsettings.json` en `service-api-csharp.API`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=servicedb;Username=postgres;Password=tu_password"
  }
}
```

### 3. Aplicar migraciones

```bash
cd service-api-csharp.API
dotnet ef database update
```

### 4. Ejecutar la aplicación

```bash
dotnet run
```

La API estará disponible en:
- **HTTP**: `http://localhost:5000`
- **HTTPS**: `https://localhost:5001`
- **Swagger**: `https://localhost:5001/swagger`

## 🐳 Docker

### Ejecutar con Docker Compose

```bash
docker-compose up -d
```

Esto iniciará:
- API en `http://localhost:8080`
- PostgreSQL en `localhost:5432`
- pgAdmin en `http://localhost:5050` (opcional)

### Construir imagen Docker

```bash
docker build -t service-api-csharp .
```

## 📚 Documentación

- [Estructura del Proyecto](docs/Architecture/PROJECT_STRUCTURE.md)
- [Diagrama de Arquitectura](docs/Architecture/ARCHITECTURE_DIAGRAM.md)
- [Guía de Desarrollo](docs/Guides/DEVELOPMENT_GUIDE.md) (próximamente)
- [API Documentation](docs/API/API_DOCUMENTATION.md) (próximamente)

## 🧪 Pruebas

### Ejecutar todas las pruebas

```bash
dotnet test
```

### Ejecutar pruebas con cobertura

```bash
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=opencover
```

### Tipos de pruebas

- **Unit Tests**: Pruebas unitarias de lógica de negocio
- **Integration Tests**: Pruebas de integración con base de datos
- **E2E Tests**: Pruebas de extremo a extremo

## 📦 Paquetes NuGet Principales

```xml
<!-- Application Layer -->
<PackageReference Include="MediatR" Version="12.2.0" />
<PackageReference Include="AutoMapper" Version="12.0.1" />
<PackageReference Include="FluentValidation" Version="11.9.0" />

<!-- Infrastructure Layer -->
<PackageReference Include="Microsoft.EntityFrameworkCore" Version="8.0.0" />
<PackageReference Include="Npgsql.EntityFrameworkCore.PostgreSQL" Version="8.0.0" />
<PackageReference Include="Serilog.AspNetCore" Version="8.0.0" />

<!-- API Layer -->
<PackageReference Include="Swashbuckle.AspNetCore" Version="6.5.0" />
<PackageReference Include="Microsoft.AspNetCore.Authentication.JwtBearer" Version="8.0.0" />
```

## 🔧 Scripts Útiles

### Crear nueva migración

```bash
dotnet ef migrations add NombreMigracion --project service-api-csharp.Infrastructure --startup-project service-api-csharp.API
```

### Revertir migración

```bash
dotnet ef database update PreviousMigration --project service-api-csharp.Infrastructure --startup-project service-api-csharp.API
```

### Limpiar y reconstruir

```bash
dotnet clean
dotnet build
```

## 🌟 Características

- ✅ Clean Architecture con separación de capas
- ✅ CQRS con MediatR
- ✅ Validación automática con FluentValidation
- ✅ Mapeo automático con AutoMapper
- ✅ Documentación con Swagger/OpenAPI
- ✅ Autenticación JWT
- ✅ Logging estructurado con Serilog
- ✅ Manejo global de excepciones
- ✅ Paginación y filtrado
- ✅ Soft Delete
- ✅ Auditoría automática (CreatedAt, UpdatedAt)
- ✅ Unit of Work pattern
- ✅ Repository pattern
- ✅ Specification pattern
- ✅ Domain Events

## 📝 Convenciones de Código

### Nomenclatura

- **Entidades**: `Product`, `User`, `Order`
- **Interfaces**: `IProductRepository`, `IEmailService`
- **DTOs**: `ProductDto`, `CreateProductDto`
- **Commands**: `CreateProductCommand`, `UpdateProductCommand`
- **Queries**: `GetProductByIdQuery`, `GetProductsListQuery`
- **Handlers**: `CreateProductCommandHandler`, `GetProductByIdQueryHandler`
- **Validators**: `CreateProductCommandValidator`

### Organización

- Organización vertical por **Features** (no por tipo de archivo)
- Cada feature contiene sus Commands, Queries, DTOs y Validators
- Código compartido en carpetas `Common`

## 🤝 Contribución

1. Fork el proyecto
2. Crea una rama para tu feature (`git checkout -b feature/AmazingFeature`)
3. Commit tus cambios (`git commit -m 'Add some AmazingFeature'`)
4. Push a la rama (`git push origin feature/AmazingFeature`)
5. Abre un Pull Request

## 📄 Licencia

Este proyecto está bajo la Licencia MIT. Ver el archivo [LICENSE](LICENSE) para más detalles.

## 👥 Autores

- **Tu Nombre** - *Desarrollo inicial* - [tu-usuario](https://github.com/tu-usuario)

## 🙏 Agradecimientos

- [Clean Architecture](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html) por Robert C. Martin
- [CQRS Pattern](https://martinfowler.com/bliki/CQRS.html) por Martin Fowler
- [Domain-Driven Design](https://www.domainlanguage.com/ddd/) por Eric Evans

## 📞 Contacto

- Email: tu-email@ejemplo.com
- LinkedIn: [tu-perfil](https://linkedin.com/in/tu-perfil)
- GitHub: [@tu-usuario](https://github.com/tu-usuario)

---

⭐ Si este proyecto te fue útil, considera darle una estrella en GitHub!
