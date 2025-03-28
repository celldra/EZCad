using System.Collections.Generic;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using GallagherCommands.Shared;

namespace GallagherCommands.Server.Commands
{
    public class AopCommand : ServerCommandBase
    {
        [Command("aop")]
        public override void RunCommand(int source, List<object> args, string raw)
        {
            if (!Players.TryGetPlayer(source, out var player)) return;

            if (!API.IsPlayerAceAllowed(player.Handle, "GCMD.Commands") ||
                !API.IsPlayerAceAllowed(player.Handle, "GCMD.Commands.AOP"))
            {
                ThrowNoPermission(player);
                return;
            }
            
            if (HasNoArguments(args))
            {
                SendErrorMessage(player,
                    "You need to type in something to set the AOP as, example: /aop <new aop>");
                return;
            }
            
            var newAop = string.Join(" ", args);
            MemoryStorage.DefaultAop = newAop;

            TriggerClientEvent("GCMD:UpdateAOP", MemoryStorage.DefaultAop);
        }
    }
}