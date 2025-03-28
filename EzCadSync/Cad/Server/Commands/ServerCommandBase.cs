using System;
using System.Collections.Generic;
using CitizenFX.Core;
using EzCadSync.Api;
using EzCadSync.Server.Utils;
using EzCadSync.Shared.Models;

namespace EzCadSync.Server.Commands;

public abstract class ServerCommandBase : BaseScript, IDisposable
{
    protected readonly ApiService Api = new();
    protected readonly SyncConfiguration Configuration = ConfigurationLoader.LoadConfiguration();

    public void Dispose()
    {
        Api?.Dispose();
    }

    protected void ThrowNoPermission(Player player)
    {
        SendChatMessage(player, "You don't have permission to run this command!");
    }

    protected void SendChatMessage(Player player, string data)
    {
        TriggerClientEvent(player, "chat:addMessage", new
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