using CitizenFX.Core;
using CitizenFX.Core.Native;
using EzCadSync.Api.Exceptions;

namespace EzCadSync.Server.Events;

public class CreateCriminalRecordEvent : BaseApiEvent
{
    [EventHandler("EZCad:CreateRecord")]
    public async void Handle([FromSource] Player player, int offenderId, string action, string offence)
    {
        try
        {
            if (!API.IsPlayerAceAllowed(player.Handle, "EZCad.CreateRecord"))
            {
                TriggerClientEvent(player, "EZCad:EmergencyNotify", "Create criminal record",
                    "You do not have permission to use this command!");
                return;
            }

            var targetPlayer = Players[offenderId];
            if (targetPlayer is null)
            {
                TriggerClientEvent(player, "EZCad:EmergencyNotify", "Create criminal record",
                    "Player isn't online at the moment");
                return;
            }

            var licenseId = player.Identifiers["license"];
            var targetLicenseId = targetPlayer.Identifiers["license"];

            await Api.CreateCriminalRecordAsync(licenseId, targetLicenseId, action, offence);

            TriggerClientEvent(player, "EZCad:EmergencyNotify", "Create criminal record",
                "Record has been created successfully!");
        }
        catch (ApiException ex)
        {
            TriggerClientEvent(player, "EZCad:EmergencyNotify", "Create criminal record", ex.Message);
        }
        finally
        {
            Dispose();
        }
    }
}