using System.Collections.Concurrent;
using EzCadSync.Api.Models;
using SyncConfiguration = EzCadSync.Shared.Models.SyncConfiguration;

namespace EzCadSync.Server;

public static class MemoryStorage
{
    /// <summary>
    ///     Holds all CAD authorized players, this allows us to save their states and revoke ace perms when needed
    /// </summary>
    public static readonly ConcurrentDictionary<string, string[]> AuthorizedPlayers = new();

    /// <summary>
    ///     Holds all currently authorized CAD identities, this allows us to cache and easily retrieve data without contacting
    ///     the API again
    /// </summary>
    public static readonly ConcurrentDictionary<string, Identity> AuthorizedIdentities = new();

    /// <summary>
    ///     Holds all currently authorized CAD identities, this allows us to cache and easily retrieve data without contacting
    ///     the API again
    /// </summary>
    public static readonly ConcurrentDictionary<string, PlayerIdentity> OnDutyIdentities = new();

    /// <summary>
    ///     Holds the sync configuration, this prevents unnecessary I/O operations
    /// </summary>
    public static SyncConfiguration Configuration;
}