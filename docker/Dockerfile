# =============================
# Etapa 1: Restore
# =============================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS restore
WORKDIR /src

# Copiamos solo los csproj para aprovechar la cache
COPY Api/Api.csproj Api/
COPY Application/Application.csproj Application/
COPY Domain/Domain.csproj Domain/
COPY Infrastructure/Infrastructure.csproj Infrastructure/

# Restauramos dependencias
RUN dotnet restore Api/Api.csproj


# =============================
# Etapa 2: Build
# =============================
FROM restore AS build

# Copiamos el resto del código
COPY . .

# Build en Release sin publicar todavía
RUN dotnet build Api/Api.csproj -c Release -o /app/build --no-restore


# =============================
# Etapa 3: Publish
# =============================
FROM build AS publish

RUN dotnet publish Api/Api.csproj -c Release -o /app/publish \
    --no-restore \
    /p:UseAppHost=false


# =============================
# Etapa 4: Runtime
# =============================
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final

WORKDIR /app
EXPOSE 8080

COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "Api.dll"]
    