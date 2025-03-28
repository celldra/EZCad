using CitizenFX.Core;
using CitizenFX.Core.Native;
using EzCadSync.Api.Exceptions;

namespace EzCadSync.Server.Events;

public class GetIdentityEvent : BaseApiEvent
{
    [EventHandler("EZCad:GetIdentity")]
    public async void Handle([FromSource] Player player, int id)
    {
        try
        {
            if (!API.IsPlayerAceAllowed(player.Handle, "EZCad.GetIdentity"))
            {
                TriggerClientEvent(player, "EZCad:EmergencyNotify", "Get identity",
                    "You do not have permission to use this command!");
                return;
            }

            var targetPlayer = Players[id];
            if (targetPlayer is null)
            {
                TriggerClientEvent(player, "EZCad:EmergencyNotify", "Get identity",
                    "Player isn't online at the moment");
                return;
            }

            var licenseId = targetPlayer.Identifiers["license"];

            var response = await Api.GetIdentityAsync(licenseId);

            if (response?.Success != true) throw new ApiException(response?.Message ?? string.Empty);

            TriggerClientEvent(player, "EZCad:EmergencyNotify", "Get identity",
                $"Name: <C>{response.Identity.LastName}, {response.Identity.FirstName}</C>" +
                $"\nSex: {response.Identity.Sex}" +
                $"\nDOB: {response.Identity.DateOfBirth}" +
                $"\nBirth place: {response.Identity.BirthPlace}" +
                $"\nRegistered vehicles: {response.Vehicles.Length}");
        }
        catch (ApiException ex)
        {
            TriggerClientEvent(player, "EZCad:EmergencyNotify", "Get identity", ex.Message);
        }
        finally
        {
            Api.Dispose();
        }
    }
}