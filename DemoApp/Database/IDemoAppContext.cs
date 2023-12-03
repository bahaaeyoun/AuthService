using Microsoft.EntityFrameworkCore.Infrastructure;

namespace DemoApp.Database;

public interface IDemoAppContext
{
    int SaveChanges();
    DatabaseFacade Database { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}