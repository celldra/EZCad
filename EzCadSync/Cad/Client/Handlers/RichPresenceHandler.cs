using System;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;

namespace EzCadSync.Client.Handlers;

public class RichPresenceHandler : BaseScript
{
    private static bool _isSetup;

    [Tick]
    public async Task HandleAsync()
    {
        if (MemoryStorage.Configuration is null) return;
        if (MemoryStorage.Configuration.RichPresenceConfiguration is {IsEnabled: true})
        {
            // Do rich presence because it's enabled

            if (!_isSetup)
            {
                // Perform actions that are only really triggered once
                API.SetDiscordAppId(MemoryStorage.Configuration.RichPresenceConfiguration.ClientId.ToString());
                API.SetDiscordRichPresenceAction(0, MemoryStorage.Configuration.RichPresenceConfiguration.ActionText,
                    MemoryStorage.Configuration.RichPresenceConfiguration.ActionUrl);
                API.SetDiscordRichPresenceAsset(MemoryStorage.Configuration.RichPresenceConfiguration.AssetName);
                API.SetDiscordRichPresenceAssetText($"ID: {Game.Player.ServerId}");

                _isSetup = true;
            }

            // Get the current location
            var currentPosition = API.GetEntityCoords(Game.PlayerPed.Handle, true);

            // Get the street name
            var streetName = World.GetStreetName(currentPosition);

            // Set the presence to whatever
            API.SetRichPresence(string.Format(MemoryStorage.Configuration.RichPresenceConfiguration.State, streetName,
                Game.Player.Name));

            // Now we sleep for a bit

            await Delay((int) TimeSpan.FromSeconds(2).TotalMilliseconds);

            // Return to prevent the infinite tick sleep from happening
            return;
        }

        // Because we know it's disabled and won't be enabled till script reboot, we can sleep forever (kind of, this is just over a week + 1/4)
        await Delay(999999999);
    }
}