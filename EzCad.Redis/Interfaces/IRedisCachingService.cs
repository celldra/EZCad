namespace EzCad.Redis.Interfaces;

public interface IRedisCachingService
{
    /// <summary>
    ///     Sets a record of data in the database cache, the record data will be serialized as JSON
    /// </summary>
    /// <param name="recordId">The ID of the record, this is appended to the end of the prepared record ID</param>
    /// <param name="data">The data to set</param>
    /// <param name="absoluteExpireTime">The expiration time of the data no matter what</param>
    /// <param name="unusedExpireTime">
    ///     The unused expiration time, this will be the time it takes for the data to expire after
    ///     it is not used
    /// </param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <typeparam name="T">The type of data to serialize</typeparam>
    /// <returns></returns>
    Task SetRecordAsync<T>(string recordId, T data, TimeSpan? absoluteExpireTime = null,
        TimeSpan? unusedExpireTime = null, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Gets or sets a record in the database cache. If the record does not exist, it will be created and returned,
    ///     otherwise it will be fetched from the cache and returned
    /// </summary>
    /// <param name="recordId">
    ///     The ID of the record, this is appended to the end of the prepared record ID. This will also be
    ///     used to create the record if it does not exist
    /// </param>
    /// <param name="getData">The function that will be called to get the data</param>
    /// <param name="noCache">Set to true to completely ignore the statement and just return <paramref name="getData"/></param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <typeparam name="T">The type of data to serialize</typeparam>
    /// <returns>The data that was requested from the database cache</returns>
    Task<T?> GetOrSetRecordAsync<T>(string recordId, Func<T> getData, bool noCache = false,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Gets or sets a record in the database cache. If the record does not exist, it will be created and returned,
    ///     otherwise it will be fetched from the cache and returned
    /// </summary>
    /// <param name="recordId">
    ///     The ID of the record, this is appended to the end of the prepared record ID. This will also be
    ///     used to create the record if it does not exist
    /// </param>
    /// <param name="getData">The asynchronous <see cref="Task" />-based function that will be called to get the data</param>
    /// <param name="noCache">Set to true to completely ignore the statement and just return <paramref name="getData"/></param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <typeparam name="T">The type of data to serialize</typeparam>
    /// <returns>The data that was requested from the database cache</returns>
    Task<T?> GetOrSetRecordAsync<T>(string recordId, Func<Task<T>> getData, bool noCache = false,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Removes a record from the database cache
    /// </summary>
    /// <param name="recordId">The ID of the record, this is appended to the end of the prepared record ID</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <typeparam name="T">The type of data to be looking for, this is because it has to fetch the data in-order to remove it</typeparam>
    /// <returns></returns>
    Task RemoveRecordAsync<T>(string recordId, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Gets a record from the database cache and deserializes the raw data into the typeparam that was given
    /// </summary>
    /// <param name="recordId">The ID of the record, this is appended to the end of the prepared record ID</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <typeparam name="T">The type of data you are looking for</typeparam>
    /// <returns></returns>
    Task<T?> GetRecordAsync<T>(string recordId, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Updates a record which already exists in the database cache, this simply removes it and adds the record back under
    ///     the same record ID with the new data
    /// </summary>
    /// <param name="recordId">The ID of the record, this is appended to the end of the prepared record ID</param>
    /// <param name="data">The new data to update the record with</param>
    /// <param name="absoluteExpireTime">The expiration time of the data no matter what</param>
    /// <param name="unusedExpireTime">
    ///     The unused expiration time, this will be the time it takes for the data to expire after
    ///     it is not used
    /// </param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <typeparam name="T">The type of data to serialize</typeparam>
    /// <returns></returns>
    Task UpdateRecordAsync<T>(string recordId, T data, TimeSpan? absoluteExpireTime = null,
        TimeSpan? unusedExpireTime = null, CancellationToken cancellationToken = default);
}