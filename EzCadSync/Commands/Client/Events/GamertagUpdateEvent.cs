using CitizenFX.Core;
using GallagherCommands.Shared;

namespace GallagherCommands.Client.Events;

public class GamertagUpdateEvent : BaseScript
{
    [EventHandler("GCMD:UpdateGamertags")]
    private void UpdateGamertags(bool newValue)
    {
        Debug.WriteLine($"Setting gamertags mem storage to {newValue}");

        if (MemoryStorage.IsShowingGamertags && !newValue)
        {
            Debug.WriteLine("Disabling gamertags");
            MemoryStorage.IsDisablingGamertags = true;
        }
        
        // Set the val on client end
        MemoryStorage.IsShowingGamertags = newValue;
    }
}