using CitizenFX.Core;
using GallagherCommands.Shared;

namespace GallagherCommands.Server.Events;

public class UpdateEvent : BaseScript
{
    [EventHandler("GCMD:Update")]
    private void RunEvent([FromSource] Player player)
    {
        TriggerClientEvent(player, "GCMD:UpdatePT", MemoryStorage.IsPeaceTimeEnabled);
        TriggerClientEvent(player, "GCMD:UpdatePriority", MemoryStorage.PriorityStatus,
            MemoryStorage.PriorityTime.ToString());
        TriggerClientEvent(player, "GCMD:UpdateAOP", MemoryStorage.DefaultAop);
    }
}