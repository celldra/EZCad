using CitizenFX.Core;
using CitizenFX.Core.Native;
using EzCadSync.Api.Exceptions;

namespace EzCadSync.Server.Events;

public class FineEvent : BaseApiEvent
{
    [EventHandler("EZCad:Fine")]
    public async void Handle([FromSource] Player player, int id, double amount, string description)
    {
        try
        {
            if (!API.IsPlayerAceAllowed(player.Handle, "EZCad.FinePlayer")) return;

            var targetPlayer = Players[id];
            if (targetPlayer is null) return;

            var targetLicenseId = targetPlayer.Identifiers["license"];
            var licenseId = player.Identifiers["license"];

            var response = await Api.FineAsync(licenseId, targetLicenseId, description, amount);

            if (response?.Success == true)
            {
                TriggerClientEvent(targetPlayer, "EZCad:EmergencyNotify", "Fine player",
                    $"You've been fined for {amount:C} by the government");
                TriggerClientEvent(player, "EZCad:EmergencyNotify", "Fine player",
                    "The fine has been completed and the criminal record has been created");

                TriggerClientEvent(targetPlayer, "EZCad:SetBalance", response.Balance);
            }
            else
            {
                throw new ApiException(response?.Message ?? string.Empty);
            }
        }
        catch (ApiException ex)
        {
            TriggerClientEvent(player, "EZCad:EmergencyNotify", "Fine player", ex.Message);
        }
        finally
        {
            Api.Dispose();
        }
    }
}