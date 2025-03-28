using System.Collections.Generic;
using CitizenFX.Core;
using CitizenFX.Core.Native;

namespace GallagherCommands.Client.Commands;

public class DeleteVehicleCommand : ClientCommandBase
{
    public DeleteVehicleCommand()
    {
        TriggerEvent("chat:addSuggestion", "/dv",
            "Deletes the vehicle you are currently driving, will not work if you're in the passenger seat");
    }

    [Command("dv")]
    public override void RunCommand(int source, List<object> args, string raw)
    {
        var currentVehicle = Game.PlayerPed.CurrentVehicle;
        if (currentVehicle is null || currentVehicle.GetPedOnSeat(VehicleSeat.Driver) != Game.PlayerPed)
        {
            SendChatMessage("You must be in a vehicle or a vehicle that you are currently driving");
            return;
        }

        var vehicleHandle = currentVehicle.Handle;
        
        API.ClearPedTasksImmediately(Game.PlayerPed.Handle);
        API.TaskEveryoneLeaveVehicle(currentVehicle.Handle);
        API.DeleteVehicle(ref vehicleHandle);
    }
}