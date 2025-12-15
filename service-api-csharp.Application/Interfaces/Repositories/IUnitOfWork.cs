namespace service_api_csharp.Application.InterfacesRepositories;

public interface IUnitOfWork : IDisposable
{
    ISystemDirectoriesRepository SystemDirectories { get; }
    IReportsRepository Reports { get; }
    Task BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();
    Task<int> SaveAsync();
}
