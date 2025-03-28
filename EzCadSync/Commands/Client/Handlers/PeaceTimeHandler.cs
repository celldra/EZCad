using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using CitizenFX.Core.UI;
using GallagherCommands.Shared;

namespace GallagherCommands.Client.Handlers;

public class PeaceTimeHandler : BaseScript
{
    private readonly int[] _disabledControls =
    {
        140, 37, 24, 12, 13, 14, 15, 16, 17, 56, 53, 54, 99, 100, 115, 116, 158, 159, 160, 161, 162, 163, 164, 165, 261,
        262, 114, 121, 140, 141, 142, 257, 263, 264, 331
    };

    [Tick]
    public async Task HandleAsync()
    {
        if (!MemoryStorage.IsPeaceTimeEnabled) return;

        Screen.DisplayHelpTextThisFrame("Peacetime is currently enabled, therefore you cannot perform any violent actions");
        
        var ped = API.PlayerPedId();

        API.DisablePlayerFiring(ped, true);
        API.SetPlayerCanDoDriveBy(ped, false);
        API.SetCurrentPedWeapon(ped, (uint) API.GetHashKey("weapon_unarmed"), true);

        ExtensionMethods.SetPedRelationship(1);

        foreach (var control in _disabledControls)
        {
            API.DisableControlAction(0, control, true);
        }

        // Delay a millisecond
        await Delay(1);
    }
}