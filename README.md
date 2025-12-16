# Service API - Clean Architecture


A robust REST API built with **.NET 9**, designed following **Clean Architecture**, **CQRS**, and **Domain-Driven Design (DDD)** principles. This service acts as a core component in a distributed system, handling business logic with high meaningfulness and maintainability.

## 🏗️ Architecture

This project strictly enforces **Clean Architecture**, ensuring a clear separation of concerns, testability, and independence from external frameworks/drivers.

### Layers Overview



1.  **Domain Layer**: The heart of the software. Contains Enterprise Logic, Entities, Enums, and Value Objects. It has **no dependencies** on other layers.
2.  **Application Layer**: Contains Application Logic. It defines Use Cases (Commands/Queries), DTOs, interfaces for repositories, and validators. It depends only on the Domain.
3.  **Infrastructure Layer**: Implements interfaces defined in Application. Handles Database access (EF Core), external API calls, File Systems, etc.
4.  **API Layer**: The entry point. Handles HTTP requests, configuration, and middleware.

### Key Patterns
*   **CQRS (Command Query Responsibility Segregation)**: Using `MediatR` to separate read and write operations.
*   **Repository Pattern**: Abstracting data access.
*   **Unit of Work**: Managing database transactions.
*   **Result Pattern**: Handling operation outcomes without exceptions for control flow.

---

## � Authentication & Security

This service utilizes a centralized authentication mechanism involving a separate Java Microservice for key management.

### How it Works

The API implements a custom **JWT (JSON Web Token)** authentication flow with dynamic public key retrieval.

1.  **JWT Verification**: The API expects a Bearer token in the `Authorization` header.
2.  **Public Key Retrieval (`JavaPublicKeyProvider`)**:
    *   The service does not store static public keys.
    *   It fetches the **JSON Web Key Set (JWKS)** from an external **Java Microservice**.
    *   **Caching**: To optimize performance and reduce network chatter, the fetched keys are cached in-memory (`IMemoryCache`) for **30 minutes**.
    *   If the cache is empty or expired, it re-fetches the keys automatically.
3.  **Token Validation (`CustomJwtHandler` & `TokenValidator`)**:
    *   The token's signature is verified against the retrieved Public Key.
    *   **Issuer** and **Audience** (if configured) are validated.
    *   Token expiration is checked.
4.  **Clean Architecture Integration**:
    *   Interfaces (`IJavaPublicKeyProvider`) are defined in the **Application Layer**.
    *   Implementation details (`HttpClient` calls, caching) reside in the **Infrastructure Layer**.

---

## � Docker and Deployment

The project is fully containerized using **Docker**, utilizing a multi-stage build process to ensure small, secure, and optimized production images.

### Dockerfile Strategy

The `Dockerfile` is optimized for caching and security:

1.  **Restore Stage**: Copies only `.csproj` files first to cache NuGet packages.
2.  **Build Stage**: Compiles the application in `Release` mode.
3.  **Publish Stage**: Publishes the artifact to a folder, removing unnecessary files.
4.  **Runtime Stage**: Uses the lightweight `mcr.microsoft.com/dotnet/aspnet:9.0` image. It exposes port `8080`.

### Running with Docker

To build and run the application container:

```bash
# Build the image
docker build -t service-api-csharp .

# Run the container
docker run -d -p 8080:8080 --name service-api service-api-csharp
```

### Docker Compose

For a complete local development environment (API + PostgreSQL), use:

```bash
docker-compose up -d
```

---

## 🚀 Getting Started

### Prerequisites
*   [.NET 9 SDK](https://dotnet.microsoft.com/download)
*   [PostgreSQL](https://www.postgresql.org/)
*   [Docker](https://www.docker.com/) (Optional)

### Installation

1.  **Clone the repository**:
    ```bash
    git clone https://github.com/your-org/service-api-csharp.git
    cd service-api-csharp
    ```

2.  **Configure Environment**:
    Update `appsettings.json` in `service-api-csharp.API` with your database credentials.

3.  **Apply Migrations**:
    ```bash
    cd service-api-csharp.API
    dotnet ef database update
    ```

4.  **Run the Application**:
    ```bash
    dotnet run
    ```
    Access Swagger UI at `http://localhost:5000/swagger`.

---

## 🧪 Testing

The project includes a comprehensive test suite.

*   **Unit Tests**: Testing individual components (Handlers, Services) with mocked dependencies.
*   **Integration Tests**: Testing the full pipeline with an in-memory or Dockerized database.

To run tests:
```bash
dotnet test
```

---

## � Project Structure

```
service-api-csharp/
├── service-api-csharp.Domain/          # Enterprise business rules
├── service-api-csharp.Application/     # Application business rules
├── service-api-csharp.Infrastructure/  # Frameworks & Drivers
├── service-api-csharp.API/             # Interface Adapters
├── tests/                              # Unit & Integration Tests
└── docker/                             # Docker configuration
```
