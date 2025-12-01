# Informe Detallado: ValueObjects Point y Polygon

## 📋 Índice
1. [Introducción](#introducción)
2. [¿Qué es un ValueObject?](#qué-es-un-valueobject)
3. [Implementación de Point](#implementación-de-point)
4. [Implementación de Polygon](#implementación-de-polygon)
5. [Configuración en la Capa de Infraestructura](#configuración-en-la-capa-de-infraestructura)
6. [Integración con NetTopologySuite](#integración-con-nettopologysuite)
7. [Conclusión](#conclusión)

---

## 🎯 Introducción

En este proyecto implementé dos **ValueObjects** (`Point` y `Polygon`) en la capa de **Domain** siguiendo los principios de **Clean Architecture**. El objetivo principal fue crear tipos de datos geoespaciales que:

1. **Mantengan el dominio puro** (sin dependencias de librerías externas como NetTopologySuite)
2. **Se mapeen correctamente** a tipos espaciales de PostgreSQL/PostGIS en la base de datos
3. **Garanticen invariantes** de negocio (validaciones)

---

## 🧩 ¿Qué es un ValueObject?

Un **ValueObject** (Objeto de Valor) es un concepto del **Domain-Driven Design (DDD)** que tiene las siguientes características:

### Características Fundamentales:

1. **Inmutabilidad**: Una vez creado, no puede cambiar
2. **Igualdad por Valor**: Dos instancias son iguales si todos sus atributos tienen los mismos valores
3. **Sin Identidad**: No tiene un ID único, se identifica por sus valores
4. **Auto-validación**: Garantiza que siempre está en un estado válido

### ¿Por qué hereda de ValueObject?

Creé una clase base abstracta `ValueObject` que implementa:

```csharp
public abstract class ValueObject
{
    protected abstract IEnumerable<object> GetEqualityComponents();
    
    public override bool Equals(object obj) { ... }
    public override int GetHashCode() { ... }
    public static bool operator ==(ValueObject left, ValueObject right) { ... }
    public static bool operator !=(ValueObject left, ValueObject right) { ... }
}
```

**Razones para heredar de ValueObject:**

1. **Evitar duplicación de código**: La lógica de comparación por valor es común
2. **Garantizar igualdad estructural**: `Point(1,2) == Point(1,2)` debe ser `true`
3. **Sobrecarga de operadores**: Permite usar `==` y `!=` de forma natural
4. **Consistencia**: Todos los ValueObjects del dominio siguen el mismo patrón

---

## 📍 Implementación de Point

### Código Completo:

```csharp
public class Point : ValueObject
{
    public double X { get; }
    public double Y { get; }
    public int Srid { get; }

    private Point(double x, double y, int srid)
    {
        X = x;
        Y = y;
        Srid = srid;
    }

    public static Point Create(double x, double y, int srid = 4326)
    {
        // Add validation if necessary (e.g., valid lat/long ranges)
        return new Point(x, y, srid);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return X;
        yield return Y;
        yield return Srid;
    }
}
```

### Análisis Detallado:

#### 1. **Propiedades Inmutables**

```csharp
public double X { get; }
public double Y { get; }
public int Srid { get; }
```

- **`{ get; }`**: Solo lectura (no tiene `set`)
- **X**: Coordenada X (Longitud en geografía)
- **Y**: Coordenada Y (Latitud en geografía)
- **Srid**: Sistema de Referencia Espacial (4326 = WGS84, el estándar GPS)

**¿Por qué inmutables?**
- Evita efectos secundarios no deseados
- Un punto en (10, 20) siempre será (10, 20)
- Thread-safe por naturaleza

#### 2. **Constructor Privado**

```csharp
private Point(double x, double y, int srid)
{
    X = x;
    Y = y;
    Srid = srid;
}
```

**¿Por qué privado?**
- **Factory Pattern**: Fuerza el uso del método `Create()`
- **Control de creación**: Permite agregar validaciones centralizadas
- **Encapsulamiento**: El dominio controla cómo se construyen los objetos

#### 3. **Método Estático Create (Factory Method)**

```csharp
public static Point Create(double x, double y, int srid = 4326)
{
    // Aquí podrías agregar validaciones:
    // if (y < -90 || y > 90) throw new ArgumentException("Latitud inválida");
    // if (x < -180 || x > 180) throw new ArgumentException("Longitud inválida");
    
    return new Point(x, y, srid);
}
```

**Ventajas del Factory Method:**
1. **Claridad**: `Point.Create(10, 20)` es más expresivo que `new Point(10, 20)`
2. **Validaciones**: Punto único para validar invariantes
3. **Flexibilidad**: Puede retornar null, lanzar excepciones o retornar tipos derivados
4. **Valor por defecto**: `srid = 4326` es el estándar geográfico global

#### 4. **GetEqualityComponents (Comparación por Valor)**

```csharp
protected override IEnumerable<object> GetEqualityComponents()
{
    yield return X;
    yield return Y;
    yield return Srid;
}
```

**¿Qué hace?**
- Define **cuáles propiedades** determinan si dos puntos son iguales
- `yield return` retorna cada componente uno por uno (iterador)

**Ejemplo práctico:**
```csharp
var p1 = Point.Create(10, 20, 4326);
var p2 = Point.Create(10, 20, 4326);
var p3 = Point.Create(10, 21, 4326);

Console.WriteLine(p1 == p2); // true (mismos valores)
Console.WriteLine(p1 == p3); // false (Y diferente)
```

**¿Por qué `protected override`?**
- `protected`: Solo visible para clases derivadas (no expuesto públicamente)
- `override`: Implementa el método abstracto de `ValueObject`

---

## 🔷 Implementación de Polygon

### Código Completo:

```csharp
public class Polygon : ValueObject
{
    public IReadOnlyList<Point> Coordinates { get; }
    public int Srid { get; }

    private Polygon(List<Point> coordinates, int srid)
    {
        Coordinates = coordinates.AsReadOnly();
        Srid = srid;
    }

    public static Polygon Create(IEnumerable<Point> points, int srid = 4326)
    {
        var pointList = points?.ToList() ?? new List<Point>();

        if (pointList.Count < 3)
        {
            throw new ArgumentException("A polygon must have at least 3 points.");
        }

        // Ensure the polygon is closed (first point equals last point)
        if (!pointList.First().Equals(pointList.Last()))
        {
            pointList.Add(pointList.First());
        }

        return new Polygon(pointList, srid);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Srid;
        foreach (var point in Coordinates)
        {
            yield return point;
        }
    }
}
```

### Análisis Detallado:

#### 1. **Propiedades Inmutables**

```csharp
public IReadOnlyList<Point> Coordinates { get; }
public int Srid { get; }
```

- **`IReadOnlyList<Point>`**: Colección de solo lectura de puntos
- **¿Por qué `IReadOnlyList` y no `List`?**
  - Previene que alguien modifique la lista después de creada
  - Refuerza la inmutabilidad del ValueObject
  - `AsReadOnly()` crea un wrapper que bloquea modificaciones

#### 2. **Constructor Privado**

```csharp
private Polygon(List<Point> coordinates, int srid)
{
    Coordinates = coordinates.AsReadOnly();
    Srid = srid;
}
```

- Convierte la lista mutable en inmutable con `AsReadOnly()`
- Solo accesible desde el método `Create()`

#### 3. **Método Create con Validaciones de Negocio**

```csharp
public static Polygon Create(IEnumerable<Point> points, int srid = 4326)
{
    var pointList = points?.ToList() ?? new List<Point>();

    // VALIDACIÓN 1: Mínimo 3 puntos
    if (pointList.Count < 3)
    {
        throw new ArgumentException("A polygon must have at least 3 points.");
    }

    // VALIDACIÓN 2: Polígono cerrado
    if (!pointList.First().Equals(pointList.Last()))
    {
        pointList.Add(pointList.First());
    }

    return new Polygon(pointList, srid);
}
```

**Validaciones implementadas:**

##### Validación 1: Mínimo 3 puntos
```csharp
if (pointList.Count < 3)
{
    throw new ArgumentException("A polygon must have at least 3 points.");
}
```
- **Regla de negocio geométrica**: Un polígono necesita al menos 3 vértices
- **Invariante**: Garantiza que nunca se cree un polígono inválido
- **Falla rápido**: Lanza excepción al intentarlo

##### Validación 2: Polígono cerrado
```csharp
if (!pointList.First().Equals(pointList.Last()))
{
    pointList.Add(pointList.First());
}
```
- **Regla de PostGIS/NetTopologySuite**: El primer punto debe ser igual al último
- **Auto-corrección**: Si no está cerrado, lo cierra automáticamente
- **Ejemplo**:
  ```csharp
  // Usuario envía:
  [Point(0,0), Point(0,10), Point(10,10)]
  
  // Se convierte en:
  [Point(0,0), Point(0,10), Point(10,10), Point(0,0)]
  ```

#### 4. **GetEqualityComponents**

```csharp
protected override IEnumerable<object> GetEqualityComponents()
{
    yield return Srid;
    foreach (var point in Coordinates)
    {
        yield return point;
    }
}
```

**¿Qué hace?**
- Primero compara el `Srid`
- Luego compara cada punto de la lista **en orden**

**Ejemplo práctico:**
```csharp
var poly1 = Polygon.Create(new[] { 
    Point.Create(0,0), Point.Create(0,10), Point.Create(10,10) 
});
var poly2 = Polygon.Create(new[] { 
    Point.Create(0,0), Point.Create(0,10), Point.Create(10,10) 
});

Console.WriteLine(poly1 == poly2); // true
```

**¿Por qué iterar todos los puntos?**
- Dos polígonos son iguales **solo si** todos sus puntos coinciden
- El orden importa: `[A, B, C]` ≠ `[B, C, A]`

---

## ⚙️ Configuración en la Capa de Infraestructura

### ¿Por qué necesitamos configuración?

**Problema:**
- El dominio usa `Point` y `Polygon` (nuestros ValueObjects)
- La base de datos usa tipos espaciales de PostGIS (`geometry(Point)`, `geometry(Polygon)`)
- **Necesitamos un traductor** entre ambos mundos

### Conversión de Point (Ejemplo en ReportConfiguration)

```csharp
builder.Property(r => r.UbicationCoordinates)
    .IsRequired()
    .HasColumnType("geometry(Point, 4326)") 
    .HasConversion(
        // DE DOMINIO -> BASE DE DATOS
        p => new NetTopologySuite.Geometries.Point(p.X, p.Y) { SRID = p.Srid },
        
        // DE BASE DE DATOS -> DOMINIO
        p => Point.Create(p.X, p.Y, p.SRID)
    );
```

#### Análisis Línea por Línea:

**1. `HasColumnType("geometry(Point, 4326)")`**
- Define el tipo de columna en PostgreSQL
- `geometry(Point, 4326)`: Tipo espacial punto con SRID 4326
- PostGIS reconoce este tipo y permite consultas espaciales

**2. `HasConversion(...)`**
- Define dos lambdas: **escritura** y **lectura**

**3. Lambda de Escritura (Dominio → BD):**
```csharp
p => new NetTopologySuite.Geometries.Point(p.X, p.Y) { SRID = p.Srid }
```
- **Entrada `p`**: Nuestro `Point` del dominio
- **Salida**: `NetTopologySuite.Geometries.Point` (que EF Core entiende)
- **Cuándo se ejecuta**: Al hacer `dbContext.SaveChanges()`

**4. Lambda de Lectura (BD → Dominio):**
```csharp
p => Point.Create(p.X, p.Y, p.SRID)
```
- **Entrada `p`**: `NetTopologySuite.Geometries.Point` de la BD
- **Salida**: Nuestro `Point` del dominio
- **Cuándo se ejecuta**: Al hacer `dbContext.Reports.ToList()`

### Conversión de Polygon (Ejemplo en CitySectorConfiguration)

```csharp
builder.Property(c => c.Area)
    .IsRequired()
    .HasColumnType("geometry(Polygon, 4326)")
    .HasConversion(
        // DE DOMINIO -> BASE DE DATOS
        poly => new NetTopologySuite.Geometries.Polygon(
            new NetTopologySuite.Geometries.LinearRing(
                poly.Coordinates.Select(c => 
                    new NetTopologySuite.Geometries.Coordinate(c.X, c.Y)
                ).ToArray()
            )
        ) { SRID = poly.Srid },
        
        // DE BASE DE DATOS -> DOMINIO
        poly => Polygon.Create(
            poly.ExteriorRing.Coordinates.Select(c => 
                Point.Create(c.X, c.Y, (int)poly.SRID)
            ), 
            (int)poly.SRID
        )
    );
```

#### Análisis de Conversión de Polygon:

**Lambda de Escritura (Dominio → BD):**
```csharp
poly => new NetTopologySuite.Geometries.Polygon(
    new NetTopologySuite.Geometries.LinearRing(
        poly.Coordinates.Select(c => 
            new NetTopologySuite.Geometries.Coordinate(c.X, c.Y)
        ).ToArray()
    )
) { SRID = poly.Srid }
```

**Paso a paso:**

1. **`poly.Coordinates`**: Lista de nuestros `Point` del dominio
2. **`.Select(c => new Coordinate(c.X, c.Y))`**: Convierte cada `Point` a `Coordinate` de NTS
3. **`.ToArray()`**: Convierte `IEnumerable` a array (requerido por NTS)
4. **`new LinearRing(...)`**: Crea el anillo exterior del polígono
5. **`new Polygon(linearRing)`**: Crea el polígono de NTS
6. **`{ SRID = poly.Srid }`**: Asigna el sistema de referencia

**Lambda de Lectura (BD → Dominio):**
```csharp
poly => Polygon.Create(
    poly.ExteriorRing.Coordinates.Select(c => 
        Point.Create(c.X, c.Y, (int)poly.SRID)
    ), 
    (int)poly.SRID
)
```

**Paso a paso:**

1. **`poly.ExteriorRing.Coordinates`**: Array de `Coordinate` de NTS
2. **`.Select(c => Point.Create(...))`**: Convierte cada `Coordinate` a nuestro `Point`
3. **`Polygon.Create(...)`**: Usa nuestro factory method
4. **`(int)poly.SRID`**: Cast porque NTS devuelve `double` pero esperamos `int`

---

## 🔗 Integración con NetTopologySuite

### ¿Qué es NetTopologySuite (NTS)?

- **Librería .NET** para operaciones geométricas y espaciales
- **Compatible con PostGIS**: PostgreSQL puede almacenar y consultar datos espaciales
- **Usado por EF Core**: Para mapear tipos espaciales

### Flujo Completo de Datos:

```
┌─────────────────────────────────────────────────────────────┐
│                    APLICACIÓN (C#)                          │
│                                                             │
│  Domain Entity:                                             │
│  var report = new Report {                                  │
│      UbicationCoordinates = Point.Create(-74.08, 4.60)     │
│  }                                                          │
└─────────────────┬───────────────────────────────────────────┘
                  │
                  │ dbContext.Reports.Add(report)
                  │ dbContext.SaveChanges()
                  │
                  ▼
┌─────────────────────────────────────────────────────────────┐
│             EF CORE + HasConversion                         │
│                                                             │
│  Convierte:                                                 │
│  Point.Create(-74.08, 4.60)                                │
│      ↓                                                      │
│  new NTS.Point(-74.08, 4.60) { SRID = 4326 }              │
└─────────────────┬───────────────────────────────────────────┘
                  │
                  │ SQL: INSERT
                  │
                  ▼
┌─────────────────────────────────────────────────────────────┐
│                  POSTGRESQL + PostGIS                       │
│                                                             │
│  Columna: ubication_coordinates geometry(Point, 4326)       │
│  Valor: POINT(-74.08 4.60)                                 │
│                                                             │
│  Permite consultas como:                                    │
│  SELECT * FROM reports                                      │
│  WHERE ST_DWithin(ubication_coordinates,                   │
│                   ST_MakePoint(-74, 4), 1000);            │
└─────────────────────────────────────────────────────────────┘
```

### Ventajas de esta Arquitectura:

#### 1. **Separación de Responsabilidades (Clean Architecture)**
- **Domain**: Lógica de negocio pura, sin dependencias
- **Infrastructure**: Detalles técnicos (BD, NTS, mapeos)

#### 2. **Testing Simplificado**
```csharp
// En Domain tests: SIN necesidad de BD
var point = Point.Create(10, 20);
Assert.Equal(10, point.X);

// En Infrastructure tests: CON BD
var report = new Report { 
    UbicationCoordinates = Point.Create(10, 20) 
};
dbContext.Reports.Add(report);
dbContext.SaveChanges();
```

#### 3. **Cambio de Proveedor**
Si mañana cambias de PostgreSQL a SQL Server:
- **Domain**: SIN cambios
- **Infrastructure**: Solo cambias la conversión

#### 4. **Consultas Espaciales**
PostGIS permite:
```sql
-- Encontrar reportes a menos de 1km de un punto
SELECT * FROM reports
WHERE ST_DWithin(
    ubication_coordinates::geography,
    ST_SetSRID(ST_MakePoint(-74.08, 4.60), 4326)::geography,
    1000 -- metros
);

-- Encontrar sectores que contienen un punto
SELECT * FROM city_sectors
WHERE ST_Contains(
    area,
    ST_SetSRID(ST_MakePoint(-74.08, 4.60), 4326)
);
```

---

## 🎓 Conclusión

### Resumen de Implementación:

| Componente | Ubicación | Responsabilidad |
|------------|-----------|-----------------|
| `ValueObject` | Domain | Clase base para igualdad por valor |
| `Point` | Domain | Representa un punto geográfico |
| `Polygon` | Domain | Representa un área geográfica |
| `ReportConfiguration` | Infrastructure | Mapea `Point` a PostGIS |
| `CitySectorConfiguration` | Infrastructure | Mapea `Polygon` a PostGIS |

### ¿Por qué esta implementación es correcta?

✅ **Cumple Clean Architecture**: Domain no depende de Infrastructure  
✅ **Invariantes garantizadas**: Polígonos siempre tienen ≥3 puntos  
✅ **Inmutabilidad**: Thread-safe y predecible  
✅ **Testing**: Domain se puede probar sin BD  
✅ **Consultas espaciales**: PostGIS aprovecha índices GIST  
✅ **Expresividad**: `Point.Create(x, y)` es claro y autocumentado  

### Próximos Pasos Posibles:

1. **Agregar más validaciones** en `Point.Create()`:
   ```csharp
   if (y < -90 || y > 90) 
       throw new ArgumentException("Latitud inválida");
   ```

2. **Métodos de dominio útiles**:
   ```csharp
   public double DistanceTo(Point other) { ... }
   public bool IsWithinBounds(Polygon area) { ... }
   ```

3. **Índices espaciales en EF Core**:
   ```csharp
   builder.HasIndex(r => r.UbicationCoordinates)
       .HasMethod("gist");
   ```

---

**Autor**: Antigravity AI  
**Fecha**: 30 de Noviembre de 2025  
**Proyecto**: CityHelp Service API
