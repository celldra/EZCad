using System;
using System.Collections.Generic;
using System.Linq;
using CitizenFX.Core;
using EzCadSync.Api.Exceptions;

namespace EzCadSync.Server.Commands;

public class LinkCadCommand : ServerCommandBase
{
    public LinkCadCommand()
    {
        Debug.WriteLine("Link CAD event constructed");
    }

    [Command("link-cad")]
    public override async void RunCommand(int source, List<object> args, string raw)
    {
        var player = Players[source];

        try
        {
            if (HasNoArguments(args) || !Guid.TryParse(args.FirstOrDefault()?.ToString(), out var guid))
            {
                SendChatMessage(player, "Invalid ID supplied, please make sure you put the correct ID");
                return;
            }

            var id = guid.ToString();
            var licenseId = player.Identifiers["license"];

            var response = await Api.LinkCadAsync(licenseId, id);
            SendChatMessage(player, response.Message);
        }
        catch (ApiException ex)
        {
            SendChatMessage(player, ex.Message);
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.ToString());
        }
    }
}