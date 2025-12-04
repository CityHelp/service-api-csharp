using NetTopologySuite.Geometries;
using service_api_csharp.Application.InterfacesRepositories;
using service_api_csharp.Infrastructure.Persistence;

namespace service_api_csharp.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;
    private readonly GeometryFactory _geometryFactory;
    private ISystemDirectoriesRepository _systemDirectories;

    public UnitOfWork(AppDbContext context, GeometryFactory geometryFactory)
    {
        _context = context;
        _geometryFactory = geometryFactory;
    }

    public ISystemDirectoriesRepository SystemDirectories => 
        _systemDirectories ??= new SystemDirectoriesRepository(_context, _geometryFactory);

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
