using System;
using System.Collections.Generic;
using CitizenFX.Core;
using CitizenFX.Core.Native;

namespace EzCadSync.Client.NuiEvents;

public class GetVehicleEvent : BaseScript
{
    public GetVehicleEvent()
    {
        API.RegisterNuiCallbackType("getVehicle");

        EventHandlers["__cfx_nui:getVehicle"] += new Action<IDictionary<string, object>, CallbackDelegate>(
            (data, callback) =>
            {
                var targetPed = 0;

                if (!API.IsPedInAnyVehicle(Game.PlayerPed.Handle, false))
                {
                    callback(new
                    {
                        success = false,
                        message = "NotInVehicle"
                    });
                    return;
                }

                var playerVehicle = Game.PlayerPed.CurrentVehicle;
                if (playerVehicle.ClassType != VehicleClass.Emergency)
                {
                    callback(new
                    {
                        success = false,
                        message = "NotInVehicle"
                    });
                    return;
                }

                var closestPlayer = GetClosestPlayer(playerVehicle.Handle);

                // We've got to get the closest ped
                if (closestPlayer is null)
                {
                    callback(new
                    {
                        success = false,
                        message = "No player is close enough to be targeted"
                    });
                    return;
                }

                targetPed = API.GetPlayerPed(closestPlayer.Handle);

                // Check if they are event in a vehicle + get the vehicle
                var vehicle = API.GetVehiclePedIsIn(targetPed, false);
                if (!API.IsPedInAnyVehicle(targetPed, false) || vehicle == 0)
                {
                    callback(new
                    {
                        success = false,
                        message = "Player not in vehicle"
                    });
                    return;
                }

                var license = API.GetVehicleNumberPlateText(vehicle);
                var model = API.GetDisplayNameFromVehicleModel((uint) API.GetEntityModel(vehicle));
                var displayModel = API.GetLabelText(model);
                var speed = API.GetEntitySpeed(vehicle);

                var vehInfo = new
                {
                    success = true,
                    model = string.IsNullOrWhiteSpace(displayModel) ? model : displayModel,
                    licensePlate = license,
                    speed = Math.Round(speed * 2.236936f)
                };

                callback(vehInfo);
            });
    }

    private Player? GetClosestPlayer(int ignoreHandle)
    {
        var closestDistance = -1f;
        Player? closestPlayer = null;
        var ped = API.GetPlayerPed(-1);
        var pedCoords = API.GetEntityCoords(ped, false);

        foreach (var player in Players)
        {
            var target = API.GetPlayerPed(player.Handle);
            if (target == ped) continue;

            var targetCoords = API.GetEntityCoords(target, false);
            var distance = API.GetDistanceBetweenCoords(targetCoords.X, targetCoords.Y, targetCoords.Z, pedCoords.X,
                pedCoords.Y, pedCoords.Z, true);

            if (player.Character.CurrentVehicle is not null &&
                player.Character.CurrentVehicle.Handle == ignoreHandle) continue;

            if (player.Character.CurrentVehicle is not null &&
                player.Character.CurrentVehicle.ClassType == VehicleClass.Emergency) continue;

            if (closestDistance != -1 && !(closestDistance > distance)) continue;

            closestPlayer = player;
            closestDistance = distance;
        }

        return closestPlayer;
    }
}