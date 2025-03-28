using CitizenFX.Core;
using CitizenFX.Core.Native;

namespace EzCadSync.Client.Events;

public class DrawEmergencyEvent : BaseScript
{
    [EventHandler("EZCad:DrawEmergency")]
    private async void DrawEmergency(float x, float y, float z)
    {
        // Add the blip to the map
        var blip = API.AddBlipForRadius(x, y, z, 100.0f);

        API.SetBlipHighDetail(blip, true);
        API.SetBlipColour(blip, 1);
        API.SetBlipAlpha(blip, 128);
        
        // Now we wait for 30 seconds, then delete the blip
        await Delay(30 * 1000);
        
        API.RemoveBlip(ref blip);
    }
}