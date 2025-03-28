using System.Collections.Generic;
using System.Linq;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using GallagherCommands.Client.Handlers;

namespace GallagherCommands.Client.Commands;

public class PostalCommand : ClientCommandBase
{
    public PostalCommand()
    {
        TriggerEvent("chat:addSuggestion", "/postal", "Draws a route to the specified postal code", new[]
        {
            new {name = "code", help = "The postal code to draw a route to"}
        });
        
    }
    [Command("postal")]
    public override void RunCommand(int source, List<object> args, string raw)
    {
        if (HasNoArguments(args))
        {
            SendChatMessage("You need to specify the postal code to draw a route to!");
            return;
        }

        var code = args.FirstOrDefault()?.ToString();

        var postal = PostalHandler.PostalList?.Find(x => x.Code == code);
        if (postal is null)
        {
            SendChatMessage("Could not find that postal code!");
            return;
        }
        
        API.SetNewWaypoint(postal.X, postal.Y);
    }
}