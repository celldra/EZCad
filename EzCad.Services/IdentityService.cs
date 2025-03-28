using EzCad.Database;
using EzCad.Database.Entities;
using EzCad.Redis.Interfaces;
using EzCad.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EzCad.Services;

public class IdentityService : IIdentityService
{
    private readonly EzCadDataContext _dataContext;
    private readonly IRedisCachingService _redis;

    public IdentityService(EzCadDataContext dataContext, IRedisCachingService redis)
    {
        _dataContext = dataContext;
        _redis = redis;
    }

    public async Task<List<Identity>> GetIdentitiesAsync(User user, bool noCache = false,
        CancellationToken cancellationToken = default)
    {
        return (await _redis.GetOrSetRecordAsync($"{user.LicenseId ?? user.Id}_identities",
            async () =>
            {
                return await _dataContext.Identities.Where(x => x.HostUser.Id == user.Id)
                    .ToListAsync(cancellationToken);
            }, noCache, cancellationToken))!;
    }

    public async Task<Identity?> GetIdentityAsync(User user, string id, bool noCache = false,
        CancellationToken cancellationToken = default)
    {
        return await _redis.GetOrSetRecordAsync($"identity_{id}", async () =>
        {
            return await _dataContext.Identities.SingleOrDefaultAsync(x => x.Id == id && x.HostUser.Id == user.Id,
                cancellationToken);
        }, noCache, cancellationToken);
    }

    public async Task<Identity?> GetIdentityAsync(string id, bool noCache = false,
        CancellationToken cancellationToken = default)
    {
        return await _redis.GetOrSetRecordAsync($"identity_{id}",
            async () =>
            {
                return await _dataContext.Identities.SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
            }, noCache, cancellationToken);
    }


    public async Task<Identity?> GetPrimaryIdentityAsync(User user, bool noCache = false,
        CancellationToken cancellationToken = default)
    {
        return await _redis.GetOrSetRecordAsync($"{user.LicenseId ?? user.Id}_primaryIdentity", async () =>
        {
            return await _dataContext.Identities.SingleOrDefaultAsync(x => x.HostUser.Id == user.Id && x.IsPrimary,
                cancellationToken);
        }, noCache, cancellationToken);
    }

    public async Task<Identity?> CreateIdentityAsync(User user, Identity identity,
        CancellationToken cancellationToken = default)
    {
        identity.HostUser = user;

        await _dataContext.AddAsync(identity, cancellationToken);
        await _dataContext.SaveChangesAsync(cancellationToken);

        return identity;
    }

    public async Task UpdateIdentityAsync(User user, string identityId, Identity newIdentity,
        CancellationToken cancellationToken = default)
    {
        var identity = await GetIdentityAsync(user, identityId, true, cancellationToken);
        if (identity is null) return;

        identity.IsPrimary = newIdentity.IsPrimary;
        identity.BirthPlace = newIdentity.BirthPlace;
        identity.FirstName = newIdentity.FirstName;
        identity.LastName = newIdentity.LastName;
        identity.DateOfBirth = newIdentity.DateOfBirth;
        identity.Sex = newIdentity.Sex;

        await _redis.UpdateRecordAsync($"identity_{identity.Id}", identity,
            cancellationToken: cancellationToken);

        await _dataContext.SaveChangesAsync(cancellationToken);

        await _redis.RemoveRecordAsync<List<Identity>>($"{user.LicenseId ?? user.Id}_identities", cancellationToken);
    }

    public async Task<(User?, Identity?)> GetPrimaryIdentityByLicense(string licenseId, bool noCache = false,
        CancellationToken cancellationToken = default)
    {
        licenseId = licenseId.Replace("license:", string.Empty);
        var user = await _dataContext.Users.SingleOrDefaultAsync(x => x.LicenseId == licenseId, cancellationToken);
        if (user is null) return (null, null);

        var identity = await _redis.GetOrSetRecordAsync($"{licenseId}_primaryIdentity", async () =>
        {
            return await _dataContext.Identities.SingleOrDefaultAsync(x => x.HostUser.Id == user.Id && x.IsPrimary,
                cancellationToken);
        }, noCache, cancellationToken);

        return identity is null ? (null, null) : (user, identity);
    }

    public async Task DeleteIdentityAsync(User user, string identityId, CancellationToken cancellationToken = default)
    {
        var identity = await GetIdentityAsync(user, identityId, true, cancellationToken);
        if (identity is null) return;

        _dataContext.Remove(identity);
        await _dataContext.SaveChangesAsync(cancellationToken);

        // Update identities
        await _redis.RemoveRecordAsync<List<Identity>>($"{user.LicenseId ?? user.Id}_identities", cancellationToken);

        // If the ID was primary, remove the primary ID record
        if (identity.IsPrimary)
            await _redis.RemoveRecordAsync<Identity>($"{user.LicenseId ?? user.Id}_primaryIdentity",
                cancellationToken);
    }
}