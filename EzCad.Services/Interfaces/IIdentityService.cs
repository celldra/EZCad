using EzCad.Database.Entities;

namespace EzCad.Services.Interfaces;

public interface IIdentityService
{
    Task<List<Identity>> GetIdentitiesAsync(User user, bool noCache = false,
        CancellationToken cancellationToken = default);

    Task<Identity?> GetIdentityAsync(User user, string id, bool noCache = false,
        CancellationToken cancellationToken = default);
    
    Task<Identity?> GetIdentityAsync(string id, bool noCache = false,
        CancellationToken cancellationToken = default);

    Task<Identity?> GetPrimaryIdentityAsync(User user, bool noCache = false,
        CancellationToken cancellationToken = default);

    Task<Identity?> CreateIdentityAsync(User user, Identity identity,
        CancellationToken cancellationToken = default);

    Task UpdateIdentityAsync(User user, string identityId, Identity newIdentity,
        CancellationToken cancellationToken = default);

    Task<(User?, Identity?)> GetPrimaryIdentityByLicense(string licenseId, bool noCache = false,
        CancellationToken cancellationToken = default);

    Task DeleteIdentityAsync(User user, string identityId, CancellationToken cancellationToken = default);
}