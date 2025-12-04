namespace service_api_csharp.Application.InterfacesRepositories;

public interface IUnitOfWork : IDisposable
{
    ISystemDirectoriesRepository SystemDirectories { get; }
    Task<int> SaveChangesAsync();
}
