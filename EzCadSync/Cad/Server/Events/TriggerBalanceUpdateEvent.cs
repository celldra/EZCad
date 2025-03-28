using CitizenFX.Core;

namespace EzCadSync.Server.Events;

public class TriggerBalanceUpdateEvent : BaseScript
{
    [EventHandler("EZCad:TriggerBalanceUpdate")]
    public void Handle([FromSource] Player player)
    {
        var licenseId = player.Identifiers["license"];
        if (!MemoryStorage.AuthorizedIdentities.ContainsKey(licenseId))
        {
            Debug.WriteLine($"User {player.Name} license ID {licenseId} does not exist in identity store");
            return;
        }
        
        var identity = MemoryStorage.AuthorizedIdentities[licenseId];
        Debug.WriteLine($"Sending balance to client {player.Name} ${identity.Balance}");

        TriggerClientEvent(player, "EZCad:SetBalance", identity.Balance);
    }
}