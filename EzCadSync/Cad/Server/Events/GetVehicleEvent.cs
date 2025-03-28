using CitizenFX.Core;
using CitizenFX.Core.Native;
using EzCadSync.Api.Exceptions;

namespace EzCadSync.Server.Events;

public class GetVehicleEvent : BaseApiEvent
{
    [EventHandler("EZCad:GetVehicle")]
    public async void GetVehicle([FromSource] Player player, string license, NetworkCallbackDelegate callback)
    {
        try
        {
            if (!API.IsPlayerAceAllowed(player.Handle, "EZCad.CreateRecord")) return;

            var licenseId = player.Identifiers["license"];

            var response = await Api.GetVehicleAsync(licenseId, license);

            await callback(response);
        }
        catch (ApiException ex)
        {
            // ignored.
            await callback(ex.Response);
        }
        finally
        {
            Api.Dispose();
        }
    }
}