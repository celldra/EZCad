using CitizenFX.Core;
using Newtonsoft.Json;

namespace EzCadSync.Server.Events;

public class TriggerConfigurationUpdateEvent : BaseScript
{
    [EventHandler("EZCad:TriggerConfigurationUpdate")]
    public void Handle([FromSource] Player player)
    {
        Debug.WriteLine("Triggering configuration update");

        TriggerClientEvent(player, "EZCad:UpdateConfiguration",
            JsonConvert.SerializeObject(MemoryStorage.Configuration));
    }
}