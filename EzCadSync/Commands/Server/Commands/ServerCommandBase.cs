using System;
using System.Collections.Generic;
using CitizenFX.Core;

namespace GallagherCommands.Server.Commands;

public abstract class ServerCommandBase : BaseScript
{
    protected void ThrowNoPermission(Player player)
    {
        SendErrorMessage(player, "You don't have permission to run this command!");
    }

    public void SendErrorMessage(Player player, string data)
    {
        TriggerClientEvent(player, "chat:addMessage",
            new
            {
                multiline = true,
                color = new[] {255, 255, 255},
                args = new[] {"System", data}
            });
    }

    protected bool HasNoArguments(List<object> args)
    {
        return args.Count <= 0;
    }

    public virtual void RunCommand(int source, List<object> args, string raw)
    {
        throw new NotImplementedException();
    }
}