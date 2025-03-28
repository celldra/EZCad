using CitizenFX.Core;
using CitizenFX.Core.Native;

namespace EzCadSync.Client.Events;

public class PlayerSpawnedEvent : BaseScript
{
    [EventHandler("playerSpawned")]
    public async void OnPlayerSpawned()
    {
        if (MemoryStorage.Configuration is { ShouldManuallyShutdownLoadscreen: false }) return;

        Debug.WriteLine("Shutting down loadscreen");
        API.ShutdownLoadingScreenNui();
    }
}