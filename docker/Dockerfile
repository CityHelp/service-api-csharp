# =============================
# Etapa 1: Restore
# =============================
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS restore
WORKDIR /src

COPY service-api-csharp.API/service-api-csharp.API.csproj service-api-csharp.API/
COPY service-api-csharp.Application/service-api-csharp.Application.csproj service-api-csharp.Application/
COPY service-api-csharp.Domain/service-api-csharp.Domain.csproj service-api-csharp.Domain/
COPY service-api-csharp.Infrastructure/service-api-csharp.Infrastructure.csproj service-api-csharp.Infrastructure/

RUN dotnet restore service-api-csharp.API/service-api-csharp.API.csproj

# =============================
# Etapa 2: Build
# =============================
FROM restore AS build
COPY . .
RUN dotnet build service-api-csharp.API/service-api-csharp.API.csproj -c Release -o /app/build --no-restore

# =============================
# Etapa 3: Publish
# =============================
FROM build AS publish
RUN dotnet publish service-api-csharp.API/service-api-csharp.API.csproj -c Release -o /app/publish --no-restore /p:UseAppHost=false

# =============================
# Etapa 4: Runtime
# =============================
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

COPY --from=publish /app/publish .

EXPOSE 8080
ENTRYPOINT ["dotnet", "service-api-csharp.API.dll"]
    