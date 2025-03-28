using CitizenFX.Core;
using CitizenFX.Core.Native;

namespace GallagherCommands.Server.Events;

public class BroadcastEvent : BaseScript
{
    [EventHandler("GCMD:Broadcast")]
    public void RunEvent([FromSource] Player player, string message)
    {
        Debug.WriteLine("Broadcast fired");

        if (!API.IsPlayerAceAllowed(player.Handle, "GCMD.Commands")) return;

        TriggerClientEvent("chat:addMessage",
            new
            {
                multiline = true,
                color = new[] {255, 255, 255},
                args = new[] {"System", message}
            });
    }
}