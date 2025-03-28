using System.Collections.Generic;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using GallagherCommands.Shared;

namespace GallagherCommands.Server.Commands;

public class ShowGamertagsCommand : ServerCommandBase
{
    [Command("showgamertags")]
    public override void RunCommand(int source, List<object> args, string raw)
    {
        Debug.WriteLine("Show gamertags invoked");
        
        var player = Players[source];
        if (!API.IsPlayerAceAllowed(player!.Handle, "GCMD.ShowGamertags"))
        {
            Debug.WriteLine("Ace not allowed for showing gamertags");
            
            ThrowNoPermission(player);
            return;
        }
        
        // Now we do the thing
        
        // Negate the current value
        MemoryStorage.IsShowingGamertags = !MemoryStorage.IsShowingGamertags;
        
        // Update value on the specific client
        TriggerClientEvent(player, "GCMD:UpdateGamertags", MemoryStorage.IsShowingGamertags);
    }
}