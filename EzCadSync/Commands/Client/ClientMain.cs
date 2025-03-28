using System;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using GallagherCommands.Client.Commands;
using GallagherCommands.Configuration.Models;
using GallagherCommands.Shared;

namespace GallagherCommands.Client;

public class ClientMain : BaseScript
{
    private readonly CommandsConfiguration? _configuration;

    public ClientMain()
    {
        Debug.WriteLine("Cmon you know, let's initialize every client command");

        _configuration = ConfigurationManager.Load();
        if (_configuration is null)
        {
            _configuration = new CommandsConfiguration();
            Debug.WriteLine("Invalid GallagherCommands configuration detected, please notify the server owner!");
        }

        TryRegisterCommand("suicide", s => { _ = new SuicideCommand(); });

        TryRegisterCommand("respawn", s =>
        {
            TriggerEvent("chat:addSuggestion", $"/{s}",
                "Respawns your character ignoring whether you're dead or not");
        });
        
        TryRegisterCommand("911", s =>
        {
            TriggerEvent("chat:addSuggestion", $"/{s}",
                "Puts a call into emergency services with your location and specified details", new[]
                {
                    new
                    {
                        name = "description",
                        help =
                            "A brief description on what has happened or happening that has caused you to make the call"
                    }
                });
        });
        
        TryRegisterCommand("me", s =>
        {
            TriggerEvent("chat:addSuggestion", $"/{s}",
                "States that you're doing something for players around you", new[]
                {
                    new
                    {
                        name = "action",
                        help =
                            "The action you're performing"
                    }
                });
        });

        TryRegisterCommand("dav", s =>
        {
            TriggerEvent("chat:addSuggestion", $"/{s}", "Deletes all unoccupied vehicles", new[]
            {
                new {name = "time", help = "The amount of seconds to wait before deleting the vehicles"}
            });
        });

        TryRegisterCommand("revive",
            s => { TriggerEvent("chat:addSuggestion", $"/{s}", "Revives you if you're dead"); });

        TryRegisterCommand("ooc", s =>
        {
            TriggerEvent("chat:addSuggestion", $"/{s}", "Says something in chat out-of-character", new[]
            {
                new {name = "message", help = "The message to say out of character"}
            });
        });

        TryRegisterCommand("aop", s =>
        {
            TriggerEvent("chat:addSuggestion", $"/{s}", "Sets the area of play for every player to another area", new[]
            {
                new {name = "area", help = "The new area of play to use"}
            });
        });
        
        TryRegisterCommand("announce", s =>
        {
            TriggerEvent("chat:addSuggestion", $"/{s}", "Sends an announcement to every player in the server", new[]
            {
                new {name = "message", help = "The message to announce to everyone"}
            });
        });
        
        TriggerEvent("chat:addSuggestion", "/e", "Starts an emote on your player", new[]
        {
            new {name = "emote", help = "The emote to play"}
        });
        
        TriggerEvent("chat:addSuggestion", "/emote", "Starts an emote on your player", new[]
        {
            new {name = "emote", help = "The emote to play"}
        });
        
        
        TriggerEvent("chat:addSuggestion", "/showgamertags", "Shows gamertags above other player heads");
        TriggerEvent("chat:addSuggestion", "/emotes", "Lists every emote available in the chat");
        TriggerEvent("chat:addSuggestion", "/emotemenu", "Shows a friendly emote menu to pick emotes to use");
        TriggerEvent("chat:addSuggestion", "/say", "Says something in character globally", new[]
        {
            new {name = "message", help = "The message to say"}
        });
        
        // Register the key mappings
        API.RegisterKeyMapping("showgamertags", "Toggles gamertags", "KEYBOARD", "Z");

        Debug.WriteLine("Done");
    }

    private void TryRegisterCommand(string name, Action<string> constructor)
    {
        if (_configuration is null || _configuration.DisabledCommands.Contains(name)) return;

        constructor(name);
    }
}