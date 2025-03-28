using System;
using System.Collections.Generic;
using System.IO;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using GallagherCommands.Configuration.Models;
using Newtonsoft.Json;

namespace GallagherCommands.Client;

public static class SpeedlimitConfigurationManager
{
    public static List<Postal>? Load()
    {
        try
        {
            var data = API.LoadResourceFile(API.GetCurrentResourceName(), "speedlimit.json");
            var json = JsonConvert.DeserializeObject<List<Postal>>(data);

            return json;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Exception: {ex}");
            return null;
        }
    }
}