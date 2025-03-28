using CitizenFX.Core;
using CitizenFX.Core.Native;
using EzCadSync.Api.Exceptions;

namespace EzCadSync.Server.Events;

public class SendMoneyEvent : BaseApiEvent
{
    [EventHandler("EZCad:SendMoney")]
    public async void Handle([FromSource] Player player, int id, double amount, string description)
    {
        try
        {
            if (!API.IsPlayerAceAllowed(player.Handle, "EZCad.SendMoney")) return;

            var targetPlayer = Players[id];
            if (targetPlayer is null) return;

            var targetLicenseId = targetPlayer.Identifiers["license"];
            var licenseId = player.Identifiers["license"];

            var response = await Api.SendMoneyAsync(licenseId, targetLicenseId, description, amount);

            if (response?.Success != true) throw new ApiException(response?.Message ?? string.Empty);

            TriggerClientEvent(targetPlayer, "EZCad:BankNotify",
                $"You've received {amount:C} from {player.Name}!",
                $"Your new balance is {response.TargetBalance:C}");
            TriggerClientEvent(player, "EZCad:BankNotify", $"You've sent {amount:C} to {targetPlayer.Name}!",
                $"Your new balance is {response.Balance:C}");

            TriggerClientEvent(player, "EZCad:SetBalance", response.Balance);
            TriggerClientEvent(targetPlayer, "EZCad:SetBalance", response.TargetBalance);
        }
        catch (ApiException ex)
        {
            TriggerClientEvent(player, "EZCad:BankNotify", "Failed to complete transaction", ex.Message);
        }
        finally
        {
            Api.Dispose();
        }
    }
}