using EzCadSync.Api.Models;

namespace EzCadSync.Api;

public static class MemoryStorage
{
    /// <summary>
    ///     Holds the sync configuration, this prevents unnecessary I/O operations
    /// </summary>
    public static SyncConfiguration Configuration;
}