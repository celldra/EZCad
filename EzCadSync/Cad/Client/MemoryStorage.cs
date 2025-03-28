using System.Collections.Generic;
using EzCadSync.Shared.Models;

namespace EzCadSync.Client;

public static class MemoryStorage
{
    /// <summary>
    ///     Holds the balance of the current client user
    /// </summary>
    public static double Balance = 0;

    /// <summary>
    ///     Holds the sync configuration, this prevents unnecessary I/O operations
    /// </summary>
    public static SyncConfiguration? Configuration;

    /// <summary>
    ///     Holds all currently authorized CAD identities, this allows us to cache and easily retrieve data without contacting
    ///     the API again
    /// </summary>
    public static Dictionary<string, PlayerIdentity> OnDutyIdentities = new();
}