// using NetTopologySuite.Algorithm;
// using service_api_csharp.Domain.ValueObjects;
//
// namespace service_api_csharp.Infrastructure;
//
// public class Utilities
// {
//     // ⚠️ Colócalo en algún lugar de tu proyecto Infrastructure (ej. una clase de utilidades)
//
// // Convierte tú Value Object de Dominio (AreaGeografica) al tipo de Base de Datos (NTS Polygon)
//     private static NetTopologySuite.Geometries.Polygon AreaGeograficaToNtsPolygon(Area area)
//     {
//         if (area == null) return null;
//
//         // 1. Crear una secuencia de coordenadas NTS (recuerda que un anillo exterior es una lista de puntos)
//         // NTS usa (Longitud, Latitud) en lugar de (Latitud, Longitud)
//         var coordinates = area.Puntos
//             .Select(p => new NetTopologySuite.Geometries.Coordinate(p.Longitud, p.Latitud))
//             .ToArray();
//
//         // 2. Crear un LineString que represente el anillo exterior del polígono
//         var exteriorRing = new NetTopologySuite.Geometries.LinearRing(coordinates);
//
//         // 3. Crear el Polígono
//         // Necesitas un GeometryFactory para especificar el SRID (4326)
//         var factory = NetTopologySuite.Geometries.GeometryFactory.Default; 
//         factory.SRID = area.Srid; 
//
//         // Los agujeros interiores (InteriorRings) serían null o una colección de LineStrings vacía
//         return factory.CreatePolygon(exteriorRing, null); 
//     }
//
// // Convierte el tipo de Base de Datos (NTS Polygon) a tu Value Object de Dominio (AreaGeografica)
//     private static Area NtsPolygonToAreaGeografica(NetTopologySuite.Geometries.Polygon ntsPolygon)
//     {
//         if (ntsPolygon == null) return null;
//
//         // 1. Obtener las coordenadas del anillo exterior
//         var coordenadasDominio = ntsPolygon.ExteriorRing.Coordinates
//             .Select(c => new Ubicacion(c.Y, c.X)) // NTS es (X=Long, Y=Lat); Dominio es (Lat, Long)
//             .ToList();
//
//         // 2. Usar el método de fábrica del VO para crear el objeto inmutable y validado
//         return Area.Create(coordenadasDominio);
//     }
// }