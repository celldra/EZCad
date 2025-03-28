using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace EzCad.Database.Factories;

// This is for design-time, not production use. Solely for migrating the tables correctly on my host PC.
/// <inheritdoc />
public class EzCadDataContextFactory : IDesignTimeDbContextFactory<EzCadDataContext>
{
    public EzCadDataContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<EzCadDataContext>()
#if DEBUG
            .UseNpgsql("Host=127.0.0.1;Database=EzCad;Username=postgres;Password=password")
#else
            .UseNpgsql("Host=127.0.0.1;Database=EzCad;Username=postgres;Password=")
#endif
            .UseLazyLoadingProxies();

        return new EzCadDataContext(optionsBuilder.Options);
    }
}