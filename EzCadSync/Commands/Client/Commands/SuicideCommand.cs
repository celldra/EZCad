using System.Collections.Generic;
using CitizenFX.Core;
using CitizenFX.Core.Native;

namespace GallagherCommands.Client.Commands;

public class SuicideCommand : ClientCommandBase
{
    public SuicideCommand()
    {
        TriggerEvent("chat:addSuggestion", "/suicide",
            "End your rein of terror by killing yourself");
    }

    [Command("suicide")]
    public override async void RunCommand(int source, List<object> args, string raw)
    {
        var ped = Game.PlayerPed;

        if (!Game.PlayerPed.IsInVehicle() && !Game.PlayerPed.IsRagdoll)
        {
            // This determines what to use to perform the animation
            var weapon = "pistol";
            var weaponType = string.Empty;

            if (API.HasPedGotWeapon(ped.Handle, (uint) API.GetHashKey("weapon_pistol"), false))
                weaponType = "weapon_pistol";

            if (API.HasPedGotWeapon(ped.Handle, (uint) API.GetHashKey("weapon_combatpistol"), false))
                weaponType = "weapon_combatpistol";

            if (string.IsNullOrWhiteSpace(weaponType))
            {
                weapon = "pill";
                weaponType = "weapon_unarmed";
            }

            API.SetCurrentPedWeapon(ped.Handle, (uint) API.GetHashKey(weaponType), true);
            await PlayAnimationFromDictionaryAsync(ped, "MP_SUICIDE", weapon);
        }

        Game.PlayerPed.Ragdoll();
        Game.PlayerPed.Kill();
    }
}