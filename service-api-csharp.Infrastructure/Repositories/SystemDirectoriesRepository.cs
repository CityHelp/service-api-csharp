using System.Globalization;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using service_api_csharp.Application.DTOs;
using service_api_csharp.Application.InterfacesRepositories;
using service_api_csharp.Application.Services;
using service_api_csharp.Domain.Entities;
using service_api_csharp.Infrastructure.Persistence;

namespace service_api_csharp.Infrastructure.Repositories;

public class SystemDirectoriesRepository : ISystemDirectoriesRepository
{
    private readonly AppDbContext _context;
    private readonly GeometryFactory _geometryFactory;

    public SystemDirectoriesRepository(AppDbContext context,  GeometryFactory geometryFactory)
    {
        _context = context;
        _geometryFactory = geometryFactory;
    }
    
    public async Task<ICollection<EmergencySite>> GetEmergencySitesNearUbication(UbicationUserDto ubication)
    {
        var userLocation = _geometryFactory.CreatePoint(
            new Coordinate(
                double.Parse(ubication.Longitude, CultureInfo.InvariantCulture),
                double.Parse(ubication.Latitude, CultureInfo.InvariantCulture)
            )
        );

        // Ensure SRID matches the stored geometry to avoid translation issues
        if (userLocation.SRID == 0)
        {
            userLocation.SRID = 4326;
        }

        var closestPerCategory = await _context.EmergencySite
            .Include(e => e.Category) // navegación
            .Select(e => new
            {
                Site = e,
                Geometry = EF.Property<Point>(e, nameof(EmergencySite.UbicationCoordinates))
            })
            .Where(x => x.Geometry != null)
            .Select(x => new
            {
                x.Site,
                Distance = EF.Functions.Distance(x.Geometry!, userLocation)
            })
            .GroupBy(x => x.Site.CategoryId)
            .Select(g => g
                .OrderBy(x => x.Distance)
                .Select(x => x.Site)
                .FirstOrDefault())
            .ToListAsync();
        
        return closestPerCategory;
    }   
}