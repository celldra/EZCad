using CitizenFX.Core;
using EzCadSync.Api.Exceptions;

namespace EzCadSync.Server.Events;

public class UpgradeRankEvent : BaseApiEvent
{
    [EventHandler("EZCad:UpgradeRank")]
    public async void Handle([FromSource] Player player, int rankId)
    {
        try
        {
            Debug.WriteLine($"Upgrade rank called to {rankId}");

            var licenseId = player.Identifiers["license"];

            var response = await Api.UpgradeRankAsync(licenseId, rankId);

            if (response?.Success != true) throw new ApiException(response?.Message ?? string.Empty);

            TriggerClientEvent(player, "EZCad:BankNotify", "Rank upgrade", response.Message);
        }
        catch (ApiException ex)
        {
            TriggerClientEvent(player, "EZCad:BankNotify", "Rank upgrade", ex.Message);
        }
        finally
        {
            Api.Dispose();
        }
    }
}