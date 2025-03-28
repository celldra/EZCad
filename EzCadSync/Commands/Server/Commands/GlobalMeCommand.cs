using System.Collections.Generic;
using CitizenFX.Core;

namespace GallagherCommands.Server.Commands;

public class GlobalMeCommand : ServerCommandBase
{
    [Command("gme")]
    public override void RunCommand(int source, List<object> args, string raw)
    {
        var player = Players[source];
        
        if (HasNoArguments(args))
        {
            SendErrorMessage(player, "You need to specify something to say out-of-character!");
            return;
        }

        var message = string.Join(" ", args);
        
        // We have to not use the base method since we need to modify the first argument
        TriggerClientEvent("chat:addMessage", new
        {
            multiline = true,
            color = new[] {255, 255, 255},
            args = new[] {$"[Global Me] {player?.Name}", $"*{message}*"}
        });
    }
}