using System;
using System.Collections.Generic;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using EzCadSync.Shared.Responses;
using Newtonsoft.Json;
using Vehicle = EzCadSync.Client.Models.Vehicle;

namespace EzCadSync.Client.NuiEvents;

public class GetVehicleRegistrationEvent : BaseScript
{
    public GetVehicleRegistrationEvent()
    {
        API.RegisterNuiCallbackType("getVehicleRegistration");

        EventHandlers["__cfx_nui:getVehicleRegistration"] += new Action<IDictionary<string, object>, CallbackDelegate>(
            (data, callback) =>
            {
                if (!data.TryGetValue("license", out var licenseObj))
                {
                    callback(new
                    {
                        success = false,
                        message = "Invalid license plate"
                    });

                    Debug.WriteLine("No license found");

                    return;
                }

                var license = licenseObj.ToString();

                void Callback(string content)
                {
                    if (content.Contains("\"success\": false"))
                    {
                        callback(new
                        {
                            success = false,
                            message = "Vehicle is not registered!"
                        });

                        return;
                    }

                    var vehicle = JsonConvert.DeserializeObject<Vehicle>(content);
                    var errorResponse = JsonConvert.DeserializeObject<ErrorResponse>(content);

                    if (vehicle is null || !string.IsNullOrWhiteSpace(errorResponse?.Message))
                    {
                        callback(new
                        {
                            success = false,
                            message = "Vehicle is not registered!"
                        });

                        return;
                    }

                    callback(new
                    {
                        hostIdentity = new
                        {
                            firstName = vehicle.HostIdentity?.FirstName,
                            lastName = vehicle.HostIdentity?.LastName
                        },
                        vehicle.LicensePlate,
                        vehicle.Manufacturer
                    });
                }

                TriggerServerEvent("EZCad:GetVehicle", license, (Action<string>) Callback);
            });
    }
}