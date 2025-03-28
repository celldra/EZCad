using System.Collections.Generic;
using System.Linq;
using CitizenFX.Core;
using CitizenFX.Core.Native;

namespace GallagherCommands.Server.Commands
{
    public class ReviveCommand : ServerCommandBase
    {
        [Command("revive")]
        public override void RunCommand(int source, List<object> args, string raw)
        {
            if (!Players.TryGetPlayer(source, out var player)) return;

            var targetId = source;

            if (!HasNoArguments(args))
                // Get first arg
                if (!int.TryParse(args.FirstOrDefault()?.ToString(), out targetId))
                    SendErrorMessage(player, "The ID of the player to revive must be a number!");

            if (!Players.TryGetPlayer(targetId, out var targetPlayer))
                SendErrorMessage(player, "The specified player could not be found!");
            
            Debug.WriteLine($"{player?.Name} attempted to revive {targetPlayer?.Name}");

            var bypass = API.IsPlayerAceAllowed(player.Handle, "GCMD.ReviveTimerBypass") || source != targetId;
            
            TriggerClientEvent(targetPlayer, "GCMD:Revive", targetId, bypass);
        }
    }
}