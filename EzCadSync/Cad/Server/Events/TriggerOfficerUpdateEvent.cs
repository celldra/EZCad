using CitizenFX.Core;
using CitizenFX.Core.Native;
using Newtonsoft.Json;

namespace EzCadSync.Server.Events;

public class TriggerOfficerUpdateEvent : BaseScript
{
    [EventHandler("EZCad:TriggerOfficerUpdate")]
    public void Handle([FromSource] Player player)
    {
        if (!API.IsPlayerAceAllowed(player.Handle, "EZCad.CreateRecord")) return;

        Debug.WriteLine("Triggering officer update");

        TriggerClientEvent(player, "EZCad:UpdateOfficers",
            JsonConvert.SerializeObject(MemoryStorage.OnDutyIdentities));
    }
}