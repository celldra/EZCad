using System;
using System.Collections.Generic;
using System.IO;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using GallagherCommands.Configuration.Models;
using Newtonsoft.Json;

namespace GallagherCommands.Shared;

public static class ConfigurationManager
{
    public static CommandsConfiguration? Load()
    {
        try
        {
            var data = API.LoadResourceFile(API.GetCurrentResourceName(), "config.json");
            var json = JsonConvert.DeserializeObject<CommandsConfiguration>(data);

            return json;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Exception: {ex}");
            return null;
        }
    }

    public static void CreateDefault()
    {
        // This function is generally for debugging
        var config = new CommandsConfiguration
        {
            AnnouncementDuration = 10,
            DisabledCommands = new List<string>
            {
                "announce"
            },
            RagdollKey = 'u',
            RespawnInterval = 30,
            DefaultAop = "Sandy Shores",
            RespawnCoOrdinates = new CoOrdinates
            {
                X = 1828.43,
                Y = 3693.01,
                Z = 34.3
            },
            Strings = new Dictionary<string, string>
            {
                {"peacetime_enabled", "~g~Enabled"},
                {"peacetime_disabled", "~r~Disabled"},
                {"priority_cooldown_text", "~r~{0} ~w~minutes"},
                {"priority_cooldown_disabled", "~r~Inactive"},
                {"priority_cooldown_active", "~g~Active"},
                {"priority_cooldown_hold", "~b~On hold"}
            }
        };

        var json = JsonConvert.SerializeObject(config, Formatting.Indented);
        File.WriteAllText(Path.Combine("configuration", "config.json"), json);
    }
}