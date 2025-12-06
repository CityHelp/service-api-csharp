using System.Globalization;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using service_api_csharp.Application.DTOs;
using service_api_csharp.Application.InterfacesRepositories;
using service_api_csharp.Domain.Entities;
using service_api_csharp.Infrastructure.Persistence;



namespace service_api_csharp.Infrastructure.Repositories;

public class SystemDirectoriesRepository : ISystemDirectoriesRepository
{
    private readonly AppDbContext _context;

    public SystemDirectoriesRepository(AppDbContext context,  GeometryFactory geometryFactory)
    {
        _context = context;
    }
    
    public async Task<ICollection<EmergencySite>> GetEmergencySitesNearUbication(UbicationUserDto ubication)
    {
        var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);
        var userLocation = geometryFactory.CreatePoint(new Coordinate(
            double.Parse(ubication.Longitude, CultureInfo.InvariantCulture),
            double.Parse(ubication.Latitude, CultureInfo.InvariantCulture)));
        
        var result = await _context.EmergencySite
            .Include(e => e.Category)
            .Select(e => new
            {
                Site = e,
                Distance = EF.Property<Point>(e, "UbicationCoordinates").Distance(userLocation)
            })
            .GroupBy(x => x.Site.CategoryId)
            .Select(g => g.OrderBy(x => x.Distance).First().Site)
            .ToListAsync();
        
        return result;
    }   
}