using System.Collections.Generic;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using GallagherCommands.Shared;

namespace GallagherCommands.Server.Commands
{
    public class PeaceTimeCommand : ServerCommandBase
    {

        [Command("pt")]
        public override void RunCommand(int source, List<object> args, string raw)
        {
            if (!Players.TryGetPlayer(source, out var player)) return;

            if (!API.IsPlayerAceAllowed(player.Handle, "GCMD.Commands") ||
                !API.IsPlayerAceAllowed(player.Handle, "GCMD.Commands.Peacetime"))
            {
                ThrowNoPermission(player);
                return;
            }

            MemoryStorage.IsPeaceTimeEnabled = !MemoryStorage.IsPeaceTimeEnabled;

            TriggerClientEvent("GCMD:UpdatePT", MemoryStorage.IsPeaceTimeEnabled);
        }
    }
}