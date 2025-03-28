using CitizenFX.Core;
using EzCadSync.Api.Exceptions;

namespace EzCadSync.Server.Events;

public class GetSalaryEvent : BaseApiEvent
{
    [EventHandler("EZCad:GetSalary")]
    public async void Handle([FromSource] Player player)
    {
        try
        {
            var licenseId = player.Identifiers["license"];

            var response = await Api.CollectSalaryAsync(licenseId);

            if (response?.Success != true) return;

            TriggerClientEvent(player, "EZCad:BankNotify", $"You've received {1000:C}",
                $"This is apart of your salary (or benefits)\nYour new balance is {response.Balance:C}");
            TriggerClientEvent(player, "EZCad:SetBalance", response.Balance);
        }
        catch (ApiException)
        {
            // ignored.
        }
        finally
        {
            Api.Dispose();
        }
    }
}