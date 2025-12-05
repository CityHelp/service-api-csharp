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
                double.Parse(ubication.Longitude),
                double.Parse(ubication.Latitude)
            )
        );

        // Ensure SRID matches the stored geometry to avoid translation issues
        if (userLocation.SRID == 0)
        {
            userLocation.SRID = 4326;
        }
        
        var closestPerCategory = await _context.EmergencySite
            .Include(e => e.Category) // navegación
            .GroupBy(e => e.CategoryId)
            .Select(g => g
                .OrderBy(e => EF.Functions.Distance(
                    EF.Property<Point>(e, nameof(EmergencySite.UbicationCoordinates)),
                    userLocation
                ))
                .FirstOrDefault()
            )
            .ToListAsync();
        
        return closestPerCategory;
    }   
}