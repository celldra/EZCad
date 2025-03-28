using EzCad.Redis.Interfaces;
using EzCad.Shared.Models;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace EzCad.Redis;

public static class ExtensionMethods
{
    /// <summary>
    ///     Builds a record ID with the first part of a string
    /// </summary>
    /// <param name="firstPart">The first part of the record</param>
    /// <returns>The string in the format of firstPart_yyyyMMdd_hh (UTC time)</returns>
    internal static string BuildRecordId(this string firstPart)
    {
        return $"{firstPart}_{DateTime.UtcNow:yyyyMMdd_hh}";
    }

    /// <summary>
    ///     Adds the entire StackExchange Redis library and the abstraction layer to the service collection
    /// </summary>
    /// <param name="services">The service collection to append to</param>
    /// <param name="configuration">The configuration Redis should use</param>
    /// <returns>The modified service collection</returns>
    public static IServiceCollection AddRedisCaching(this IServiceCollection services,
        Action<DatabaseConfiguration> configuration)
    {
        var invokedConfiguration = new DatabaseConfiguration();
        configuration(invokedConfiguration);
        
        services.AddStackExchangeRedisCache(options =>
        {
            var config = new ConfigurationOptions
            {
                KeepAlive = 2,
                ResolveDns = true,
                AbortOnConnectFail = true,
                ConnectTimeout = 5000,
                ConnectRetry = 16,
#if DEBUG
                IncludeDetailInExceptions = true,
                IncludePerformanceCountersInExceptions = true,
#endif
            };

            config.EndPoints.Add(invokedConfiguration.BuildRedisConnectionString());

            options.ConfigurationOptions = config;
            options.InstanceName = $"{invokedConfiguration.Name}_";
        });

        services.AddScoped<IRedisCachingService, RedisCachingService>();
        return services.Configure(configuration);
    }
}