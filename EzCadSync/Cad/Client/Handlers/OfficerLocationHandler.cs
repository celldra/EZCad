using System;
using System.Linq;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using EzCadSync.Client.Models;

namespace EzCadSync.Client.Handlers;

public class OfficerLocationHandler : BaseScript
{
    private static void SetCorrectBlipSprite(int ped, int blip)
    {
        if (API.IsPedInAnyVehicle(ped, false))
        {
            var vehicle = API.GetVehiclePedIsIn(ped, false);
            var blipSprite = BlipInfo.GetBlipSpriteForVehicle(vehicle);
            if (API.GetBlipSprite(blip) != blipSprite) API.SetBlipSprite(blip, blipSprite);
        }
        else
        {
            API.SetBlipSprite(blip, 1);
        }
    }

    [Tick]
    public async Task HandleAsync()
    {
        if (MemoryStorage.OnDutyIdentities.Count == 0) await Delay(1500);

        if (!API.DecorIsRegisteredAsType("vmenu_player_blip_sprite_id", 3))
        {
            try
            {
                API.DecorRegister("vmenu_player_blip_sprite_id", 3);
            }
            catch (Exception e)
            {
                Debug.WriteLine(
                    @"Cannot assign blip sprite ID because something else is using it, ensure that vMenu hasn't got show player blips enabled");
                Debug.WriteLine($"Error Location: {e.StackTrace}\nError info: {e.Message}");
                await Delay(1000);
            }

            while (!API.DecorIsRegisteredAsType("vmenu_player_blip_sprite_id", 3)) await Delay(0);
        }

        foreach (var p in MemoryStorage.OnDutyIdentities
                     .Select(playerIdentity => Players.FirstOrDefault(x => x.Name == playerIdentity.Value.Name))
                     .Where(p => p != null && API.NetworkIsPlayerActive(p.Handle) && p.Character != null &&
                                 p.Character.Exists()))
        {
            if (p == Game.Player) continue;

            var ped = p.Character.Handle;
            var blip = API.GetBlipFromEntity(ped);

            // if blip id is invalid.
            if (blip < 1) blip = API.AddBlipForEntity(ped);
            // only manage the blip for this player if the player is nearby
            if (p.Character.Position.DistanceToSquared2D(Game.PlayerPed.Position) < 500000 || Game.IsPaused)
            {
                // (re)set the blip color in case something changed it.
                API.SetBlipColour(blip, 0);

                // if the decorator exists on this player, use the decorator value to determine what the blip sprite should be.
                if (API.DecorExistOn(p.Character.Handle, "vmenu_player_blip_sprite_id"))
                {
                    var decorSprite = API.DecorGetInt(p.Character.Handle, "vmenu_player_blip_sprite_id");
                    // set the sprite according to the decorator value.
                    API.SetBlipSprite(blip, decorSprite);

                    // show heading on blip only if the player is on foot (blip sprite 1)
                    API.ShowHeadingIndicatorOnBlip(blip, decorSprite == 1);

                    // set the blip rotation if the player is not in a helicopter (sprite 422).
                    if (decorSprite != 422) API.SetBlipRotation(blip, (int) API.GetEntityHeading(ped));
                }
                else // backup method for when the decorator value is not found.
                {
                    // set the blip sprite using the backup method in case decorators failed.
                    SetCorrectBlipSprite(ped, blip);

                    // only show the heading indicator if the player is NOT in a vehicle.
                    if (!API.IsPedInAnyVehicle(ped, false))
                    {
                        API.ShowHeadingIndicatorOnBlip(blip, true);
                    }
                    else
                    {
                        API.ShowHeadingIndicatorOnBlip(blip, false);

                        // If the player is not in a helicopter, set the blip rotation.
                        if (!p.Character.IsInHeli) API.SetBlipRotation(blip, (int) API.GetEntityHeading(ped));
                    }
                }

                // set the player name.
                API.SetBlipNameToPlayerName(blip, p.Handle);

                // thanks lambda menu for hiding this great feature in their source code!
                // sets the blip category to 7, which makes the blips group under "Other Players:"
                API.SetBlipCategory(blip, 7);

                //N_0x75a16c3da34f1245(blip, false); // unknown

                // display on minimap and main map.
                API.SetBlipDisplay(blip, 6);
            }
            else
            {
                // hide it from the minimap.
                API.SetBlipDisplay(blip, 3);
            }
        }
    }
}