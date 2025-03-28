using System.Text.Json;
using EzCad.Redis.Interfaces;
using EzCad.Shared.Models;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace EzCad.Redis;

public class RedisCachingService : IRedisCachingService
{
    private readonly IDistributedCache _cache;
    private readonly DatabaseConfiguration _configuration;
    private readonly ILogger<RedisCachingService> _logger;

    public RedisCachingService(IDistributedCache cache, IOptions<DatabaseConfiguration> configuration,
        ILogger<RedisCachingService> logger)
    {
        _cache = cache;
        _logger = logger;
        _configuration = configuration.Value;
    }

    public async Task SetRecordAsync<T>(string recordId, T data, TimeSpan? absoluteExpireTime = null,
        TimeSpan? unusedExpireTime = null, CancellationToken cancellationToken = default)
    {
        if (!_configuration.IsEnabled) return;

        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = absoluteExpireTime ?? TimeSpan.FromSeconds(60),
            SlidingExpiration = unusedExpireTime
        };

        var json = JsonSerializer.Serialize(data);

        await _cache.SetStringAsync(recordId.BuildRecordId(), json, options, cancellationToken);
        
        _logger.LogInformation("Cached record set for {RecordId}", recordId.BuildRecordId());
    }

    public async Task<T?> GetOrSetRecordAsync<T>(string recordId, Func<T> getData, bool noCache = false,
        CancellationToken cancellationToken = default)
    {
        if (!_configuration.IsEnabled || noCache) return getData();

        var record = await GetRecordAsync<T>(recordId.BuildRecordId(), cancellationToken);
        if (record is not null)
        {
            _logger.LogInformation("Returning cached record for {RecordId}", recordId.BuildRecordId());
            return record;
        }

        record = getData();
        await SetRecordAsync(recordId.BuildRecordId(), record, null, null, cancellationToken);

        return record;
    }

    public async Task<T?> GetOrSetRecordAsync<T>(string recordId, Func<Task<T>> getData, bool noCache = false,
        CancellationToken cancellationToken = default)
    {
        if (!_configuration.IsEnabled || noCache) return await getData();

        var record = await GetRecordAsync<T>(recordId.BuildRecordId(), cancellationToken);
        if (record is not null)
        {
            _logger.LogInformation("Returning cached record for {RecordId}", recordId.BuildRecordId());
            return record;
        }

        record = await getData();
        await SetRecordAsync(recordId.BuildRecordId(), record, null, null, cancellationToken);

        return record;
    }

    public async Task UpdateRecordAsync<T>(string recordId, T data, TimeSpan? absoluteExpireTime = null,
        TimeSpan? unusedExpireTime = null, CancellationToken cancellationToken = default)
    {
        if (!_configuration.IsEnabled) return;

        // Remove existing record
        await RemoveRecordAsync<T>(recordId, cancellationToken);

        // Then set record
        await SetRecordAsync(recordId, data, absoluteExpireTime, unusedExpireTime, cancellationToken);
    }

    public async Task RemoveRecordAsync<T>(string recordId, CancellationToken cancellationToken = default)
    {
        if (!_configuration.IsEnabled) return;

        if (await GetRecordAsync<T>(recordId.BuildRecordId(), cancellationToken) is null) return;
        
        _logger.LogInformation("Removing cached record for {RecordId}", recordId.BuildRecordId());

        await _cache.RemoveAsync(recordId.BuildRecordId(), cancellationToken);
    }

    public async Task<T?> GetRecordAsync<T>(string recordId, CancellationToken cancellationToken = default)
    {
        if (!_configuration.IsEnabled) return default;

        var json = await _cache.GetStringAsync(recordId.BuildRecordId(), cancellationToken);

        return json is null ? default : JsonSerializer.Deserialize<T>(json);
    }
}