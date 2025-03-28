using CitizenFX.Core;
using CitizenFX.Core.Native;

namespace GallagherCommands.Server.Events;

public class AuthorizeEvent : BaseScript
{
    [EventHandler("GCMD:Authorize")]
    private async void RunEvent([FromSource] Player player, string acePerm,  NetworkCallbackDelegate callback)
    {
        Debug.WriteLine("Authorize ace perms fired");
        
        if (!API.IsPlayerAceAllowed(player.Handle, "GCMD.Commands") ||
            !API.IsPlayerAceAllowed(player.Handle, $"GCMD.Commands.{acePerm}"))
        {
            await callback(false);
            return;
        }

        await callback(true);
    }
}