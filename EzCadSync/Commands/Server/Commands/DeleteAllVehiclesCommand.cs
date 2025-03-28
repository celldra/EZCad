using System.Collections.Generic;
using System.Linq;
using CitizenFX.Core;
using CitizenFX.Core.Native;

namespace GallagherCommands.Server.Commands;

public class DeleteAllVehiclesCommand : ServerCommandBase
{
    [Command("dav")]
    public override async void RunCommand(int source, List<object> args, string raw)
    {
        var player = Players[source];
        
        if (!API.IsPlayerAceAllowed(player.Handle, "GCMD.Commands.Peacetime"))
        {
            TriggerClientEvent(player, "chat:addMessage",
                new
                {
                    multiline = true,
                    color = new[] { 255, 255, 255 },
                    args = new[] { "System", "You don't have permission to run this command!" }
                });
            return;
        }
        
        var waitTime = 0;
        if (!HasNoArguments(args) && !int.TryParse(args.FirstOrDefault()?.ToString(), out waitTime))
        {
            SendErrorMessage(player, "Argument must be the number of seconds to wait before deleting all vehicles!");
            return;
        }
        
        TriggerClientEvent("UL:ScreenNotify", "Warning", $"All unoccupied vehicles will be deleted in {waitTime} second(s)!");

        await Delay(waitTime * 1000);
        
        var vehicles = API.GetAllVehicles();
        if (vehicles is not List<object> vehicleObjectHandles) return;

        foreach (var handle in vehicleObjectHandles)
        {
            // Convert the handle to an int
            if (!int.TryParse(handle.ToString(), out var intHandle)) continue;
            
            if (API.IsPedAPlayer(API.GetPedInVehicleSeat(intHandle, -1))) continue;
            API.DeleteEntity(intHandle);
        }
        
        TriggerClientEvent("UL:ScreenNotify", "Warning", "All unoccupied vehicles have been deleted!");
    }
}