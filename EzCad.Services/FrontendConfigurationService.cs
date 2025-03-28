using EzCad.Database;
using EzCad.Database.Entities;
using EzCad.Redis.Interfaces;
using EzCad.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EzCad.Services;

public class FrontendConfigurationService : IFrontendConfigurationService
{
    private readonly EzCadDataContext _dataContext;
    private readonly IRedisCachingService _redis;

    public FrontendConfigurationService(EzCadDataContext dataContext, IRedisCachingService redis)
    {
        _dataContext = dataContext;
        _redis = redis;
    }

    public async Task UpdateConfigurationAsync(string? serverName = null, string? primaryHexColor = null,
        string? serverConnectUrl = null, string currency = "$", CancellationToken cancellationToken = default)
    {
        var configuration = await GetConfigurationAsync(true, cancellationToken);
        configuration.ServerName = serverName ?? configuration.ServerName;
        configuration.PrimaryHexColor = primaryHexColor ?? configuration.PrimaryHexColor;
        configuration.ConnectUrl = serverConnectUrl ?? configuration.ConnectUrl;
        configuration.Currency = currency;

        _dataContext.Update(configuration);
        await _dataContext.SaveChangesAsync(cancellationToken);

        await _redis.UpdateRecordAsync("config", configuration, cancellationToken: cancellationToken);
    }

    public async Task<Configuration> GetConfigurationAsync(bool noCache = false, CancellationToken cancellationToken = default)
    {
        await CreateIfMissingAsync(cancellationToken);

        return (await _redis.GetOrSetRecordAsync("config",
            async () => (await _dataContext.Configurations.SingleOrDefaultAsync(cancellationToken))!,
            noCache, cancellationToken))!;
    }

    private async Task CreateIfMissingAsync(CancellationToken cancellationToken = default)
    {
        if (await _dataContext.Configurations.SingleOrDefaultAsync(cancellationToken) == null)
        {
            // Now we create the default configuration
            var configuration = new Configuration();

            await _dataContext.AddAsync(configuration, cancellationToken);
            await _dataContext.SaveChangesAsync(cancellationToken);

            await _redis.UpdateRecordAsync("config", configuration, cancellationToken: cancellationToken);
        }
    }
}