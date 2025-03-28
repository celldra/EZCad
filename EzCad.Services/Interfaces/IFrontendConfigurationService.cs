using EzCad.Database.Entities;

namespace EzCad.Services.Interfaces;

public interface IFrontendConfigurationService
{
    Task UpdateConfigurationAsync(string? serverName = null, string? primaryHexColor = null,
        string? serverConnectUrl = null, string currency = "$", CancellationToken cancellationToken = default);

    Task<Configuration> GetConfigurationAsync(bool noCache = false, CancellationToken cancellationToken = default);
}