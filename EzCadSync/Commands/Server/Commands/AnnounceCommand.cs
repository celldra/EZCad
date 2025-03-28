using System.Collections.Generic;
using CitizenFX.Core;
using CitizenFX.Core.Native;

namespace GallagherCommands.Server.Commands
{
    public class AnnounceCommand : ServerCommandBase
    {
        [Command("announce")]
        public override void RunCommand(int source, List<object> args, string raw)
        {
            var player = Players[source];
            
            if (!API.IsPlayerAceAllowed(player.Handle, "GCMD.Commands") ||
                !API.IsPlayerAceAllowed(player.Handle, "GCMD.Commands.Announce"))
            {
                ThrowNoPermission(player);
                return;
            }

            if (HasNoArguments(args))
            {
                SendErrorMessage(player,
                    "You need to type in something to announce, for example: /announce <message>");
                return;
            }

            // Prepare the full message
            var message = string.Join(" ", args);
            TriggerClientEvent("UL:ScreenNotify", "Announcement", message);
        }
    }
}