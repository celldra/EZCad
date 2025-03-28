using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using Newtonsoft.Json;

namespace GallagherCommands.Client.Handlers;

public class SpeedLimitHandler : BaseScript
{
    private static readonly List<SpeedLimit>? SpeedLimits = SpeedLimitConfigurationManager.Load();
    private static string _currentStreet = string.Empty;

    [Tick]
    public async Task HandleAsync()
    {
        if (SpeedLimits is null)
        {
            Debug.WriteLine("Could not load speedlimit configuration");
            await Delay(500 * 1000);

            return;
        }

        // Delay for 500ms
        await Delay(500);

        // Get the players ped ID
        var playerPed = API.GetPlayerPed(-1);

        // Check if they're in a vehicle, if not then we don't bother with anything
        if (API.IsPedInAnyVehicle(playerPed, true))
        {
            // We need to get the street
            var playerLocation = API.GetEntityCoords(playerPed, true);
            var streetHash = (uint)0;
            var crossingHash = (uint)0;
            API.GetStreetNameAtCoord(playerLocation.X, playerLocation.Y, playerLocation.Z, ref streetHash,
                ref crossingHash);

            // Get the name from the previously gotten hash key
            var streetName = API.GetStreetNameFromHashKey(streetHash);

            // Return if the street has not changed as we don't need to do anything else
            if (_currentStreet == streetName) return;

            // Update the street
            _currentStreet = streetName;

            // Check if we can find a speedlimit for the current street
            if (SpeedLimits.Any(x => x.RoadName == _currentStreet))
            {
                // Find it in the list
                var speedLimit = SpeedLimits.SingleOrDefault(x => x.RoadName == _currentStreet);

                // Now we fire back to NUI
                var message = new
                {
                    type = "speedlimit",
                    speedlimit = $"{speedLimit!.Limit} mph"
                };

                API.SendNuiMessage(JsonConvert.SerializeObject(message));
            }
        }
    }
}