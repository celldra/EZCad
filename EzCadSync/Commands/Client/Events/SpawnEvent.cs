using CitizenFX.Core;
using CitizenFX.Core.Native;
using GallagherCommands.Configuration.Models;
using GallagherCommands.Shared;

namespace GallagherCommands.Client.Events;

public class SpawnEvent : BaseScript
{
    [EventHandler("playerSpawned")]
    public void OnPlayerSpawned()
    {
        Debug.WriteLine("Player spawned");
        
        var ped = API.GetPlayerPed(-1);
        
        API.NetworkSetFriendlyFireOption(true);
        API.SetCanAttackFriendly(ped, true, true);
    }
}