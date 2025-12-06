using NetTopologySuite.Geometries;
using service_api_csharp.Application.InterfacesRepositories;
using service_api_csharp.Infrastructure.Persistence;

namespace service_api_csharp.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;
    private readonly GeometryFactory _geometryFactory;
    public ISystemDirectoriesRepository SystemDirectories { get; }

    public UnitOfWork(AppDbContext context, GeometryFactory geometryFactory,  ISystemDirectoriesRepository systemDirectoriesRepository)
    {
        _context = context;
        _geometryFactory = geometryFactory;
        SystemDirectories = systemDirectoriesRepository;
    }

    public async Task<int> SaveAsync() => await _context.SaveChangesAsync();

    public async Task BeginTransactionAsync()
    {
        if (_context.Database.CurrentTransaction == null)
            await _context.Database.BeginTransactionAsync();
    }

    public async Task CommitTransactionAsync()
    {
        if (_context.Database.CurrentTransaction != null)
            await _context.Database.CommitTransactionAsync();
    }

    public async Task RollbackTransactionAsync()
    {
        if (_context.Database.CurrentTransaction != null)
            await _context.Database.RollbackTransactionAsync();
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
