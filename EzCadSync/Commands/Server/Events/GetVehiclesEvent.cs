using CitizenFX.Core;
using CitizenFX.Core.Native;

namespace GallagherCommands.Server.Events;

public class GetVehiclesEvent : BaseScript
{
    [EventHandler("GCMD:GetAllVehicles")]
    private async void RunEvent([FromSource] Player player, NetworkCallbackDelegate callback)
    {
        Debug.WriteLine("Get all vehicles event fired");
        
        if (!API.IsPlayerAceAllowed(player.Handle, "GCMD.Commands") ||
            !API.IsPlayerAceAllowed(player.Handle, "GCMD.Commands.DeleteVehicles"))
        {
            Debug.WriteLine("Ace not allowed for deleting vehicles");
            
            await callback(null);
            return;
        }
        
        await callback(API.GetAllVehicles());
    }
}