using System;
using System.Collections.Generic;
using System.Linq;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using GallagherCommands.Client.Handlers;

namespace GallagherCommands.Client.Commands;

public class EmergencyServicesCommand : ClientCommandBase
{
    public EmergencyServicesCommand()
    {
        Debug.WriteLine("EMS event constructed");
    }

    [Command("911")]
    public override void RunCommand(int source, List<object> args, string raw)
    {
        try
        {
            if (!args.Any())
            {
                SendChatMessage(
                    "Invalid description specified, please make sure that you specify one otherwise EMS won't know what's happening");
                return;
            }

            var description = string.Join(" ", args);

            // Get the current location
            var currentPosition = API.GetEntityCoords(Game.PlayerPed.Handle, true);

            // Get the street name
            var streetName = World.GetStreetName(currentPosition);

            // Get the closest postal
            var closestPostal = PostalHandler.CalculatePostal(Game.PlayerPed.Position.X, Game.PlayerPed.Position.Y);

            TriggerServerEvent("EZCad:CreateEmergencyReport", description, streetName, closestPostal.Item1,
                Game.PlayerPed.Position.X, Game.PlayerPed.Position.Y, Game.PlayerPed.Position.Z);
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.ToString());
        }
    }
}