# 🔐 Sistema de Validación JWT con Clean Architecture

Este documento describe la implementación del sistema de validación de tokens JWT firmados por un macroservicio de autenticación Java.

## 📋 Arquitectura

El sistema sigue los principios de **Clean Architecture**, separando las responsabilidades en capas:

```
┌─────────────────────────────────────────────────────────┐
│                    WebAPI Layer                         │
│  - JwtMiddleware: Intercepta requests y valida tokens   │
│  - Controllers: Endpoints protegidos                    │
└─────────────────────────────────────────────────────────┘
                          ↓
┌─────────────────────────────────────────────────────────┐
│                 Application Layer                       │
│  - IJavaPublicKeyProvider: Contrato para obtener llave │
│  - ITokenValidator: Contrato para validar JWT          │
└─────────────────────────────────────────────────────────┘
                          ↓
┌─────────────────────────────────────────────────────────┐
│               Infrastructure Layer                      │
│  - JavaPublicKeyProvider: Consume endpoint Java        │
│  - TokenValidator: Valida JWT con RSA                  │
└─────────────────────────────────────────────────────────┘
```

## 🚀 Componentes Implementados

### 1. **Application Layer** (Interfaces)

#### `IJavaPublicKeyProvider`
```csharp
public interface IJavaPublicKeyProvider
{
    Task<string> GetPublicKeyAsync();
}
```

#### `ITokenValidator`
```csharp
public interface ITokenValidator
{
    Task<TokenValidationResult> ValidateTokenAsync(string token);
}
```

### 2. **Infrastructure Layer** (Implementaciones)

#### `JavaPublicKeyProvider`
- Consume el endpoint del macroservicio Java
- Cachea la llave pública por 20 minutos
- Maneja errores de conexión

#### `TokenValidator`
- Convierte llave PEM a `RsaSecurityKey`
- Valida firma y expiración del JWT
- Retorna `ClaimsPrincipal` si es válido

### 3. **WebAPI Layer** (Middleware)

#### `JwtMiddleware`
- Extrae el token del header `Authorization: Bearer {token}`
- Valida el token usando `ITokenValidator`
- Asigna `context.User` si es válido
- Retorna `401 Unauthorized` si falla

## ⚙️ Configuración

### appsettings.json

```json
{
  "JavaAuthService": {
    "BaseUrl": "http://localhost:8080",
    "PublicKeyEndpoint": "/api/auth/public-key"
  }
}
```

### Variables de Entorno (.env)

```env
JAVA_AUTH_SERVICE_URL=http://localhost:8080
```

## 📦 Dependencias Instaladas

```xml
<PackageReference Include="Microsoft.IdentityModel.Tokens" Version="8.2.1" />
<PackageReference Include="System.IdentityModel.Tokens.Jwt" Version="8.2.1" />
<PackageReference Include="Microsoft.Extensions.Caching.Memory" Version="9.0.0" />
<PackageReference Include="Microsoft.Extensions.Http" Version="9.0.0" />
```

## 🔧 Registro de Servicios

En `Infrastructure/DependencyInjection.cs`:

```csharp
// Caché en memoria
services.AddMemoryCache();

// HttpClient para Java
services.AddHttpClient<IJavaPublicKeyProvider, JavaPublicKeyProvider>(client =>
{
    client.BaseAddress = new Uri(javaServiceBaseUrl);
    client.Timeout = TimeSpan.FromSeconds(30);
});

// Validador de tokens
services.AddScoped<ITokenValidator, TokenValidator>();
```

En `Program.cs`:

```csharp
// Middleware de autenticación
app.UseJwtAuthentication();
```

## 🧪 Pruebas

### Endpoint de Prueba

```http
GET /api/testauth/protected
Authorization: Bearer {tu-token-jwt}
```

**Respuesta exitosa (200):**
```json
{
  "message": "¡Acceso autorizado! 🎉",
  "user": {
    "id": "user-123",
    "name": "Juan Pérez",
    "email": "juan@example.com",
    "claims": [
      { "type": "sub", "value": "user-123" },
      { "type": "email", "value": "juan@example.com" }
    ]
  }
}
```

**Respuesta sin token (401):**
```json
{
  "error": "Unauthorized",
  "message": "Token de autenticación requerido"
}
```

## 🔄 Flujo de Autenticación

```
1. Frontend → API: Request con header "Authorization: Bearer {token}"
                    ↓
2. JwtMiddleware: Extrae el token del header
                    ↓
3. ITokenValidator: Solicita llave pública
                    ↓
4. IJavaPublicKeyProvider: 
   - Verifica caché
   - Si no existe, consume endpoint Java
   - Cachea por 20 minutos
                    ↓
5. TokenValidator:
   - Convierte PEM → RSA
   - Valida firma
   - Valida expiración
                    ↓
6. Si válido: context.User = ClaimsPrincipal
   Si inválido: 401 Unauthorized
                    ↓
7. Controller: Accede a User.Claims
```

## 🛡️ Seguridad

- ✅ Validación de firma RSA
- ✅ Validación de expiración
- ✅ Caché seguro de llave pública
- ✅ Manejo de errores robusto
- ✅ Logging de eventos de autenticación

## 📝 Notas Importantes

1. **Formato del Token**: El header debe ser `Authorization: Bearer {token}`
2. **Caché**: La llave pública se cachea por 20 minutos
3. **Timeout**: El HttpClient tiene un timeout de 30 segundos
4. **ClockSkew**: Se permite una tolerancia de 5 minutos para diferencias de reloj
5. **Logging**: Todos los eventos se registran para auditoría

## 🎯 Próximos Pasos

- [ ] Configurar `ValidateIssuer` y `ValidateAudience` según tus necesidades
- [ ] Implementar refresh tokens
- [ ] Agregar rate limiting
- [ ] Implementar circuit breaker para el servicio Java
- [ ] Agregar métricas y monitoreo

## 🤝 Integración con Java

El servicio Java debe exponer un endpoint que retorne la llave pública en formato PEM:

```java
@GetMapping("/api/auth/public-key")
public ResponseEntity<String> getPublicKey() {
    String publicKey = "-----BEGIN PUBLIC KEY-----\n" +
                       "MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEA...\n" +
                       "-----END PUBLIC KEY-----";
    return ResponseEntity.ok(publicKey);
}
```

---

**Desarrollado con ❤️ siguiendo Clean Architecture**
