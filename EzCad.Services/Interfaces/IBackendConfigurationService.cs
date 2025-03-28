using EzCad.Shared.Models;

namespace EzCad.Services.Interfaces;

public interface IBackendConfigurationService
{
    AppConfiguration Configuration { get; }
    void LoadConfiguration();
}